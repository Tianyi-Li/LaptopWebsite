using System;
using System.Net.Http;
using System.Threading.Tasks;
using Casestudy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    public class DataController : Controller
    {
        AppDbContext _ctx;
        public DataController(AppDbContext context)
        {
            _ctx = context;
        }
        public async Task<IActionResult> Index()
        {
            UtilityModel util = new UtilityModel(_ctx);
            string msg = "";
            var json = await getProductItemJsonFromWebAsync();
            try
            {
                msg = (util.loadLaptopTables(json)) ? "tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            ViewBag.LoadedMsg = msg;
            return View();
        }





        private async Task<String> getProductItemJsonFromWebAsync()
        {
            string url = "https://raw.githubusercontent.com/Tianyi-Li/info3067/master/laptops.json";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}