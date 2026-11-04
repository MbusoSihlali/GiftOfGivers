using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GiftOfGivers.Function
{
    public class TaxCertificateFunction
    {
        private readonly ILogger<TaxCertificateFunction> _logger;

        public TaxCertificateFunction(ILogger<TaxCertificateFunction> logger)
        {
            _logger = logger;
        }

        [Function("GenerateTaxCertificate")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("Processing tax certificate request for Gift of the Givers.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            DonationRequest? data = null;

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                try
                {
                    data = JsonSerializer.Deserialize<DonationRequest>(requestBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (JsonException)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid JSON payload format.");
                    return badResponse;
                }
            }

            if (data == null || string.IsNullOrWhiteSpace(data.DonorName) || data.Amount <= 0)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please submit a POST request with valid donor details JSON.");
                return badResponse;
            }

            var certificate = new
            {
                CertificateNumber = $"GOTG-TAX-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                Organization = "Gift of the Givers Foundation",
                DonorName = data.DonorName,
                TaxReferenceNumber = string.IsNullOrWhiteSpace(data.TaxNumber) ? "N/A" : data.TaxNumber,
                DonationAmount = data.Amount,
                Currency = "ZAR",
                IssuedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                Status = "Issued - Eligible for Tax Deduction"
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(certificate);
            return response;
        }
    }

    public class DonationRequest
    {
        public string? DonorName { get; set; }
        public string? TaxNumber { get; set; }
        public decimal Amount { get; set; }
    }
}