namespace Coordinator.Services
{
    using Coordinator.Models;
    using Coordinator.Models.Contexts;
    using Coordinator.Services.Abstractions;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the <see cref="TransactionService" />
    /// </summary>
    public class TransactionService(IHttpClientFactory _httpClientFactory, TwoPhaseCommitContext _context) : ITransactionService
    {
        //TwoPhaseCommitContext _context;

        //public TransactionService(TwoPhaseCommitContext context)
        //{
        //    _context = context;
        //}

        /// <summary>
        /// Defines the _orderHttpClient
        /// </summary>
        private HttpClient _orderHttpClient = _httpClientFactory.CreateClient("OrderAPI");

        /// <summary>
        /// Defines the _stockHttpClient
        /// </summary>
        private HttpClient _stockHttpClient = _httpClientFactory.CreateClient("StockAPI");

        /// <summary>
        /// Defines the _paymentHttpClient
        /// </summary>
        private HttpClient _paymentHttpClient = _httpClientFactory.CreateClient("PaymentAPI");

        /// <summary>
        /// The CreateTransactionAsync
        /// </summary>
        /// <returns>The <see cref="Task{Guid}"/></returns>
        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();

            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Enums.ReadyType.Pending,
                    TransactionState = Enums.TransactionState.Pending
                }
            });

            await _context.SaveChangesAsync();
            return transactionId;
        }

        /// <summary>
        /// The PrepareServicesAsync
        /// </summary>
        /// <param name="transactionId">The transactionId<see cref="Guid"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task PrepareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                   .Include(ns => ns.Node)
                   .Where(ns => ns.TransactionId == transactionId)
                   .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready"),
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.Unready;
                }
                catch (Exception)
                {
                    transactionNode.IsReady = Enums.ReadyType.Unready;
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// The CheckReadyServicesAsync
        /// </summary>
        /// <param name="transactionId">The transactionId<see cref="Guid"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
            => (await _context.NodeStates
                         .Where(ns => ns.TransactionId == transactionId)
                         .ToListAsync()).TrueForAll(ns => ns.IsReady == Enums.ReadyType.Ready);

        /// <summary>
        /// The CommitAsync
        /// </summary>
        /// <param name="transactionId">The transactionId<see cref="Guid"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                                    .Where(ns => ns.TransactionId == transactionId)
                                    .Include(ns => ns.Node)
                                    .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("commit"),
                        "Stock.API" => _stockHttpClient.GetAsync("commit"),
                        "Payment.API" => _paymentHttpClient.GetAsync("commit")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.TransactionState = result ? Enums.TransactionState.Done : Enums.TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// The CheckTransactionStateServicesAsync
        /// </summary>
        /// <param name="transactionId">The transactionId<see cref="Guid"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
            => (await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync()).TrueForAll(ns => ns.TransactionState == Enums.TransactionState.Done);

        /// <summary>
        /// The RollbackAsync
        /// </summary>
        /// <param name="transactionId">The transactionId<see cref="Guid"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RollbackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if (transactionNode.TransactionState == Enums.TransactionState.Done)
                        _ = await (transactionNode.Node.Name switch
                        {
                            "Order.API" => _orderHttpClient.GetAsync("rollback"),
                            "Stock.API" => _stockHttpClient.GetAsync("rollback"),
                            "Payment.API" => _paymentHttpClient.GetAsync("rollback"),
                        });

                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = Enums.TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
