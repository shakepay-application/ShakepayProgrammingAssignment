

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
        public async Task<List<NetWorth>> GetNetWorthOverTime()
        {
            List<NetWorth> netWorths = new();
            //Get API info
            List<Transaction> transactions = (await GetTransactions()).OrderBy(t => t.createdAt).ToList();
            var btcRates = await GetBTCRates();
            var ethRates = await GetETHRates();

            //Set currency amount
            decimal cad = 0m;
            decimal btc = 0m;
            decimal eth = 0m;

            foreach (var transaction in transactions)
            {
                if (transaction.direction is null)
                {
                    if (transaction.from?.currency == "BTC")
                    {
                        btc -= transaction.from?.amount ?? 0m;
                    }
                    else if (transaction.from?.currency == "ETH")
                    {
                        eth -= transaction.from?.amount ?? 0m;
                    }
                    else
                    {
                        cad -= transaction.from?.amount ?? 0m;
                    }

                    if (transaction.to?.currency == "BTC")
                    {
                        btc += transaction.to?.amount ?? 0m;
                    }
                    else if (transaction.to?.currency == "ETH")
                    {
                        eth += transaction.to?.amount ?? 0m;
                    }
                    else
                    {
                        cad += transaction.to?.amount ?? 0m;
                    }
                }
                else
                {
                    if (transaction.currency == "BTC")
                        btc += transaction.amount ?? 0m * (transaction.direction == "credit" ? 1 : -1);
                    else if (transaction.currency == "ETH")
                        eth += transaction.amount ?? 0m * (transaction.direction == "credit" ? 1 : -1);
                    else
                        cad += transaction.amount ?? 0m * (transaction.direction == "credit" ? 1 : -1);
                }
                var currentNetWorth = GetCADValue(transaction.createdAt, btc, btcRates) + GetCADValue(transaction.createdAt, eth, ethRates) + (cad);
                netWorths.Add(new NetWorth
                {
                    Date = transaction.createdAt,
                    Worth = currentNetWorth
                });
            }
            return netWorths;
        }
        private async Task<List<Transaction>> GetTransactions()
        {
            var result = await _httpClient.GetStringAsync("https://shakepay.github.io/programming-exercise/web/transaction_history.json"); 
            var listOfTransactions = JsonConvert.DeserializeObject<List<Transaction>>(result);
            return listOfTransactions;
        }
        private async Task<List<Rate>> GetBTCRates()
        {
            var result = await _httpClient.GetStringAsync("https://shakepay.github.io/programming-exercise/web/rates_CAD_BTC.json");
            var listOfRates = JsonConvert.DeserializeObject<List<Rate>>(result);
            return listOfRates;
        }
        private async Task<List<Rate>> GetETHRates()
        {
            var result = await _httpClient.GetStringAsync("https://shakepay.github.io/programming-exercise/web/rates_CAD_ETH.json");
            var listOfRates = JsonConvert.DeserializeObject<List<Rate>>(result);
            return listOfRates;
        }
            public decimal GetCADValue(DateTime currentDate, Decimal numberOfTokens, List<Rate> rates)
            {
                if (numberOfTokens == 0)
                    return 0m;
                var currentRate = rates.FirstOrDefault(r => r.createdAt.Date.Equals(currentDate.Date)) ?? rates.First();
                return numberOfTokens * currentRate?.midMarketRate ?? 0m;

            }
        }

    public record NetWorth
    {
        public DateTime Date { get; set; }  
        public decimal Worth { get; set; }
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
