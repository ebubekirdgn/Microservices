using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[Route("api/transaction")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public TransactionController(HttpClient httpClient) // Doğrudan bağımlılık enjeksiyonu
    {
        _httpClient = httpClient;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransactionRequest request)
    {
        // BankA'ya Prepare isteği gönder
        var prepareResponse = await _httpClient.PostAsJsonAsync("https://localhost:7268/api/banka/prepare", request);

        if (!prepareResponse.IsSuccessStatusCode)
        {
            return BadRequest("BankA hazırlık aşamasını reddetti!");
        }

        // BankB'ye Prepare isteği gönder
        var prepareResponseB = await _httpClient.PostAsJsonAsync("https://localhost:7226/api/bankb/prepare", request);

        if (!prepareResponseB.IsSuccessStatusCode)
        {
            return BadRequest("BankB hazırlık aşamasını reddetti!");
        }

        // BankA ve BankB Commit işlemlerini onayladıysa işlemi tamamla
        await _httpClient.PostAsync("https://localhost:7268/api/banka/commit", null);
        await _httpClient.PostAsync("https://localhost:7226/api/bankb/commit", null);

        return Ok(new { message = "Transaction committed successfully!" });
    }
}

public class TransactionRequest
{
    public int Amount { get; set; }
    public string SenderAccount { get; set; }
    public string ReceiverAccount { get; set; }
}
