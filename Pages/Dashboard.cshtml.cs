using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Info.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public List<dynamic> NewsArticles { get; set; } = new List<dynamic>();
        public string SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1; // Current page number
        public int TotalPages { get; set; } // Total number of pages
        public int TotalResults { get; set; } // Total number of results

        public async Task OnGetAsync(string searchQuery, int pageNumber = 1)
        {
            Username = User.FindFirstValue(ClaimTypes.Name);
            Email = User.FindFirstValue(ClaimTypes.Email);

            SearchQuery = searchQuery;
            PageNumber = pageNumber;
            int pageSize = 10; // Number of articles per page
            int offset = (PageNumber - 1) * pageSize; // Calculate the offset

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                try
                {
                    string apiKey = "be1137cec3e34918ba30c5ce7495ea42";  // Replace with your Bing News API key
                    string apiUrl = $"https://api.bing.microsoft.com/v7.0/news/search?q={searchQuery}&count={pageSize}&offset={offset}";

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                        var response = await client.GetStringAsync(apiUrl);

                        var newsData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                        if (newsData != null && newsData.ContainsKey("value"))
                        {
                            NewsArticles = JsonConvert.DeserializeObject<List<dynamic>>(newsData["value"].ToString());

                            // Get the total estimated number of matches
                            TotalResults = newsData.ContainsKey("totalEstimatedMatches") ? Convert.ToInt32(newsData["totalEstimatedMatches"]) : 0;

                            // Calculate total pages
                            TotalPages = (int)Math.Ceiling((double)TotalResults / pageSize);
                        }
                        else
                        {
                            ViewData["Message"] = "No articles found.";
                        }
                    }
                }
                catch
                {
                    ViewData["Message"] = "An error occurred while fetching articles. Please try again.";
                }
            }
        }
    }
}
