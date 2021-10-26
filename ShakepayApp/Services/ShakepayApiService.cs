

using Newtonsoft.Json;

namespace ShakepayApp.Services
{
    public class ShakepayApiService
    {
        private HttpClient _httpClient;

        public ShakepayApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;   
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            var result = await _httpClient.GetStringAsync("https://shakepay.github.io/programming-exercise/web/transaction_history.json"); 
            var listOfTransactions = JsonConvert.DeserializeObject<List<Transaction>>(result);
            return listOfTransactions;
        }
        public async Task<List<Rate>> GetBTCRates()
        {
            var result = await _httpClient.GetStringAsync("https://shakepay.github.io/programming-exercise/web/rates_CAD_BTC.json");
            var listOfRates = JsonConvert.DeserializeObject<List<Rate>>(result);
            return listOfRates;
        }
        public async Task<List<Rate>> GetETHRates()
        {
            var result = await _httpClient.GetStringAsync("https://shakepay.github.io/programming-exercise/web/rates_CAD_ETH.json");
            var listOfRates = JsonConvert.DeserializeObject<List<Rate>>(result);
            return listOfRates;
        }
    }

    public record Rate
    {
        public string pair { get; set; }
        public decimal midMarketRate { get; set; }
        public DateTime createdAt { get; set; }
    }
    public record Transaction
    {
        public DateTime createdAt { get; set; }
        public string currency { get; set; }
        public decimal? amount { get; set; }
        public string type { get; set; }
        public string direction { get; set; }
        public To to { get; set; }
        public From from { get; set; }
    }

    public record To
    {
        public string toAddress { get; set; }
        public string currency { get; set; }
        public decimal? amount { get; set; }

    }

    public record From
    {
        public string fromAddress { get; set; }
        public string currency { get; set; }
        public decimal? amount { get; set; }

    }

}
