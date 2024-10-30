using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

public class PublicSearchResults : PageModel
{
    public List<dynamic> NewsArticles { get; set; } = new List<dynamic>();
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalResults { get; set; }

    private readonly ILogger<PublicSearchResults> _logger;

    public PublicSearchResults(ILogger<PublicSearchResults> logger)
    {
        _logger = logger;
    }

    public async Task OnGetAsync(string searchQuery, int pageNumber = 1)
    {
        PageNumber = pageNumber;
        int pageSize = 10;  // Define how many results you want per page
        int offset = (PageNumber - 1) * pageSize;  // Calculate the offset for pagination

        try
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string apiKey = "be1137cec3e34918ba30c5ce7495ea42";  // Replace with your Bing News API key
                string apiUrl = $"https://api.bing.microsoft.com/v7.0/news/search?q={searchQuery}&count={pageSize}&offset={offset}";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                    var response = await client.GetStringAsync(apiUrl);

                    // Log full response for debugging
                    _logger.LogInformation("Full response: {Response}", response);

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
}
