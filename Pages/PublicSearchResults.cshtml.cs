using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Info.Pages.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class PublicSearchResults : PageModel
{
    public List<dynamic> NewsArticles { get; set; } = new List<dynamic>();
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }

    private readonly ILogger<PublicSearchResults> _logger;
    private readonly UserDbContext _dbContext; // Use UserDbContext

    public PublicSearchResults(ILogger<PublicSearchResults> logger, UserDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task OnGetAsync(string searchQuery, int pageNumber = 1)
    {
        PageNumber = pageNumber;
        int pageSize = 10; // Define how many results you want per page
        int offset = (PageNumber - 1) * pageSize; // Calculate the offset for pagination

        try
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string apiKey = "be1137cec3e34918ba30c5ce7495ea42"; // Your Bing News API key
                string apiUrl = $"https://api.bing.microsoft.com/v7.0/news/search?q={searchQuery}&count={pageSize}&offset={offset}";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                    var response = await client.GetStringAsync(apiUrl);

                    var newsData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    if (newsData != null && newsData.ContainsKey("value"))
                    {
                        NewsArticles = JsonConvert.DeserializeObject<List<dynamic>>(newsData["value"].ToString());

                        // Get the total estimated number of matches for pagination
                        TotalResults = newsData.ContainsKey("totalEstimatedMatches") ? Convert.ToInt32(newsData["totalEstimatedMatches"]) : 0;

                        // Calculate the total number of pages
                        TotalPages = (int)Math.Ceiling((double)TotalResults / pageSize);
                    }
                    else
                    {
                        ViewData["Message"] = "No articles found.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching articles from Bing News API.");
            ViewData["Message"] = "An error occurred while searching for articles. Please try again later.";
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
