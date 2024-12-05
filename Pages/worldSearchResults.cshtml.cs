using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Info.Pages.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Info.Pages
{
    public class worldSearchResultsModel : PageModel
    {
        private readonly ILogger<worldSearchResultsModel> _logger;

        public List<dynamic> NewsArticles { get; set; } = new List<dynamic>();
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        private readonly UserDbContext _dbContext; // Use UserDbContext


        public worldSearchResultsModel(ILogger<worldSearchResultsModel> logger, UserDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

        }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            PageNumber = pageNumber;
            int pageSize = 10; // Number of articles per page
            int offset = (PageNumber - 1) * pageSize;

            try
            {
                string apiKey = "be1137cec3e34918ba30c5ce7495ea42";
                string apiUrl = $"https://api.bing.microsoft.com/v7.0/news/search?q=breaking+world+news+OR+crisis+OR+disaster+OR+war&count={pageSize}&offset={offset}&mkt=en-US&sortBy=Date&freshness=Day";

                _logger.LogInformation($"API URL: {apiUrl}");

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation($"Raw API Response: {responseData}");

                        var newsData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

                        if (newsData != null && newsData.ContainsKey("value"))
                        {
                            var articles = JsonConvert.DeserializeObject<List<dynamic>>(newsData["value"].ToString());

                            // Sort articles by datePublished in descending order
                            NewsArticles = articles.OrderByDescending(article =>
                            {
                                DateTime date;
                                return DateTime.TryParse(article.datePublished?.ToString(), out date) ? date : DateTime.MinValue;
                            }).ToList();

                            TotalResults = newsData.ContainsKey("totalEstimatedMatches") ? Convert.ToInt32(newsData["totalEstimatedMatches"]) : NewsArticles.Count;
                            TotalPages = (int)Math.Ceiling((double)TotalResults / pageSize);
                        }
                        else
                        {
                            ViewData["Message"] = "No world news articles found.";
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"API Request failed. Status code: {response.StatusCode}, Response: {errorContent}");
                        ViewData["Message"] = "An error occurred while fetching world general news from the Bing News API.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching world general news from Bing News API.");
                ViewData["Message"] = "An error occurred. Please try again later.";
            }
        }
        public async Task<IActionResult> OnPostFollowTopicAsync(string topic)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to the login page with the topic as a return parameter
                return RedirectToPage("/Login", new { returnUrl = "/followingPage", topic });
            }

            // Retrieve the UserId from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogError("UserId claim not found.");
                return RedirectToPage("/Error");
            }

            var userId = int.Parse(userIdClaim); // Convert UserId to int

            // Save the followed topic
            var followedTopic = new FollowedTopic
            {
                UserId = userId,
                Topic = topic
            };

            _dbContext.FollowedTopics.Add(followedTopic);
            await _dbContext.SaveChangesAsync();

            TempData["Message"] = $"You are now following the topic: {topic}";

            // Redirect to the Following page
            return RedirectToPage("/followingPage");
        }
    }
}
