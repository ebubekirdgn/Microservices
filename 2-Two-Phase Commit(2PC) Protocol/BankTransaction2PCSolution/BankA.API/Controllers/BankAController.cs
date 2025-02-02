using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/banka")]
[ApiController]
public class BankAController : ControllerBase
{
    private static Dictionary<string, int> Accounts = new Dictionary<string, int>
    {
        { "A123", 5000 }  // Hesap numarası: Bakiye
    };
    private static bool Prepared = false;
    private static int TransactionAmount = 0;
    private static string Sender = "";

    [HttpPost("prepare")]
    public IActionResult Prepare([FromBody] TransactionRequest request)
    {
        if (Accounts.ContainsKey(request.SenderAccount) && Accounts[request.SenderAccount] >= request.Amount)
        {
            Prepared = true;
            TransactionAmount = request.Amount;
            Sender = request.SenderAccount;
            return Ok("VOTE COMMIT");
        }
        return BadRequest("VOTE ABORT");
    }

    [HttpPost("commit")]
    public IActionResult Commit()
    {
        if (Prepared)
        {
            Accounts[Sender] -= TransactionAmount;
            Prepared = false;
            return Ok("Commit successful!");
        }
        return BadRequest("No prepared transaction found!");
    }

    [HttpPost("abort")]
    public IActionResult Abort()
    {
        Prepared = false;
        return Ok("Transaction aborted!");
    }
}

public class TransactionRequest
{
    public int Amount { get; set; }
    public string SenderAccount { get; set; }
    public string ReceiverAccount { get; set; }
}
