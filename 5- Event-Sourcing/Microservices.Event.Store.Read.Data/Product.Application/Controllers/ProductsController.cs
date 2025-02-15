using EventStore.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Models.ViewModels;
using Shared.Events;
using Shared.Services.Abstractions;

namespace Product.Application.Controllers
{
    public class ProductsController(IEventStoreService eventStoreService) : Controller
    {
        // GET: ProductsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductVM model)
        {
           await eventStoreService.AppendToStreamAsync("products-stream", new List<EventData>
           {
               eventStoreService.GenerateEventData(new NewProductAddedEvent
               {
                   ProductId = Guid.NewGuid().ToString(),
                   ProductName = model.ProductName,
                   InitialCount = model.Count,
                   IsAvailable = model.IsAvailable,
                   InitialPrice = model.Price
               })
           });

            return RedirectToAction(nameof(Index));
        }

    }
}
