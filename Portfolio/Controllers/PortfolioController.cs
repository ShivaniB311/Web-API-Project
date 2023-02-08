using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portfolio.Data.Models;
using Portfolio.Business.Business;
using System.Net;

namespace Portfolio.Controllers
{
    
    public class PortfolioController : Controller
    {
        private readonly IPortfolioRebalance _portfolioRebalance;

        public PortfolioController(IPortfolioRebalance portfolioRebalance)
        {
            _portfolioRebalance = portfolioRebalance;
        }

        [HttpGet("rebalance")]
        [EnableCors()]
        public async Task<ActionResult<List<Stock>>> Rebalance()
        {
            return   await _portfolioRebalance.Rebalance();
        }
    }

}

