using Newtonsoft.Json;
using Portfolio.Data.Models;

namespace Portfolio.Business.Business
{
	public class StockPrice : IStockPrice
	{

        public async Task<Dictionary<string, TimeSeriesData>> getDataFromURL(HttpClient client, string symbol)
        {
            //Calling Alpha Vantage stock time series API to find current price of each stock
            string url = "https://www.alphavantage.co/query?apikey=BPYXEC00NPEI4YSY&function=TIME_SERIES_DAILY_ADJUSTED&symbol=" + symbol;

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json_data = await response.Content.ReadAsStringAsync();

                TimeSeries timeSeries = JsonConvert.DeserializeObject<TimeSeries>(json_data);
                if (timeSeries.Data != null)
                {
                    return timeSeries.Data;
                }
                else
                {
                    throw new Exception(json_data);
                }
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<PricesList> getCurrentPrices()
        {
            var SymbolList = new List<string>() { "AAPL", "THD", "CYBR", "ABB" };
            PricesList prices = new PricesList();
            Dictionary<string, double> currentPrices = new Dictionary<string, double>();
            Dictionary<string, double> todayOpenPrice = new Dictionary<string, double>();
            Dictionary<string, double> yesterdayClosedPrice = new Dictionary<string, double>();
            using (var client = new HttpClient())
            {
                foreach (var symbol in SymbolList)
                {

                    var timeSeriesDetails = await getDataFromURL(client, symbol.ToUpper());

                    //Get Adjusted Closed Price
                    var adjustedCode = timeSeriesDetails.FirstOrDefault().Value.AdjustedClose;
                    //Get Today's Open Price
                    var openPrice = timeSeriesDetails.FirstOrDefault().Value.Open;
                    //Get Yesterday's Closed Price
                    var closePrice = timeSeriesDetails.ToList().Take(2).LastOrDefault().Value.Close;

                    currentPrices.Add(symbol, adjustedCode);
                    todayOpenPrice.Add(symbol, openPrice);
                    yesterdayClosedPrice.Add(symbol, closePrice);
                }
            }
            prices.currentPrices = currentPrices;
            prices.yesterdayClosedPrice = yesterdayClosedPrice;
            prices.todayOpenPrice = todayOpenPrice;
            return prices;
        }
    }
}

