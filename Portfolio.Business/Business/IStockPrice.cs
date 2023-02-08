using System;
using Portfolio.Data.Models;

namespace Portfolio.Business.Business
{
	public interface IStockPrice
	{
        Task<Dictionary<string, TimeSeriesData>> getDataFromURL(HttpClient client, string symbol);

        Task<PricesList> getCurrentPrices();
    }
}

