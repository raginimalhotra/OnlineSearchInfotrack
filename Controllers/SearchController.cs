using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSearchInfotrack.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;


namespace OnlineSearchInfotrack.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public SearchController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(SearchModel vm)
        {
            if (ModelState.IsValid)
            {
                string keywords = Regex.Replace(vm.SearchTerm, @"\s+", "+");

                var searchResult = GetPositions(vm.Url, keywords);

                var positions = (searchResult.Result.Count > 0 ? string.Join(",", searchResult.Result) : "0");

                ViewBag.SearchResult = string.Format("Search result for the url is {0}", positions);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<int>> GetPositions(string url, string keywords)
        {

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/search?q= {keywords} &num=100");
            return FindPositions(response, url);
        }

        private List<int> FindPositions(string html, string url)
        {
            var urls = Regex.Matches(html, @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>\S+))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            List<string> validUrls = new List<string>();

            foreach (Match m in urls)
            {
                string validUrlPrefix = "/url?q=";
                if (m.Groups[1].Value.StartsWith(validUrlPrefix))
                {
                    validUrls.Add(m.Groups[1].Value);
                }
            }

            List<int> SearchResult = new List<int>();

            for (int i = 0; i < validUrls.Count; i++)
            {
                if (validUrls[i].Contains(url))
                {
                    SearchResult.Add(i + 1);
                }
            }
            return SearchResult;
        }
    }
}
