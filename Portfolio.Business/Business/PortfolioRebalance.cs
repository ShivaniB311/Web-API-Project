using Newtonsoft.Json;
using Portfolio.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Business.Business
{
    public class PortfolioRebalance : IPortfolioRebalance
    {
        //Constructor dependency injection
        private readonly IStockPrice _stockPrice;

        public PortfolioRebalance(IStockPrice stockPrice)
        {
            _stockPrice = stockPrice;
        }

        //Define the current portfolio
        private Dictionary<string, int> currentPortfolio = new Dictionary<string, int>
        {
            { "AAPL", 50 },
            { "THD", 200 },
            { "CYBR", 150 },
            { "ABB", 900 }
        };

        // Define the desired portfolio 
        private Dictionary<string, double> desiredPercentages = new Dictionary<string, double>
        {
            { "AAPL", 0.22 },
            { "THD", 0.38 },
            { "CYBR", 0.25 },
            { "ABB", 0.15 }
        };

        public async Task<List<Stock>> Rebalance()
        {
            var pricesList = await _stockPrice.getCurrentPrices();
            List<Stock> portfolios = new List<Stock>();

            //current portfolio value
            double totalValue = currentPortfolio.Sum(x => x.Value * pricesList.currentPrices[x.Key]);
            Dictionary<string, (int, double)> result = new Dictionary<string, (int, double)>();

            foreach (var stock in currentPortfolio)
            {
                Stock portfolio = new Stock();

                //initial portfolio value
                double initialValue = Math.Round(stock.Value * pricesList.currentPrices[stock.Key], 2);

                //Redistributing value based on target percentages
                double desiredValue = desiredPercentages[stock.Key] * totalValue;

                //Redistributing shares based on new value and Adjusted closed price
                int desiredShares = (int)Math.Round(desiredValue / pricesList.currentPrices[stock.Key]);

                //determine to buy or sell
                int difference = desiredShares - stock.Value;

                portfolio.Symbol = stock.Key;
                portfolio.NumberOfShares = difference;
                portfolio.PortfolioValue = initialValue;

                //
                portfolio.Percentage = Math.Round((desiredShares * pricesList.currentPrices[stock.Key] / totalValue) * 100, 2);

                //Initial number of allocated shares
                portfolio.ExistingShares = stock.Value;
                portfolio.NumberOfShareToBuy = difference > 0 ? difference : 0;
                portfolio.NumberOfShareToSell = difference > 0 ? 0 : -(difference);

                //New shares after rebalancing
                portfolio.TotalShares = stock.Value + difference;
                portfolio.TotalValue = totalValue;
                portfolio.DesiredStockValue = desiredValue;
                portfolio.AdjustedClosedPrice = pricesList.currentPrices[stock.Key];
                portfolio.TodayOpenPrice = pricesList.todayOpenPrice[stock.Key];
                portfolio.YesterdayClosePrice = pricesList.yesterdayClosedPrice[stock.Key];

                //Percentage of shares before rebalancing
                portfolio.ExistingAllocationPercentage = Math.Round(pricesList.currentPrices[stock.Key] * stock.Value * 100 / totalValue,2);

                //
                portfolio.PercentageDifferenceOfOpenandClosedPrice = ((portfolio.TodayOpenPrice - portfolio.YesterdayClosePrice) * 100 / portfolio.YesterdayClosePrice).ToString("F") + "%";
                portfolios.Add(portfolio);

            }
            return portfolios;
        }
    }
}
