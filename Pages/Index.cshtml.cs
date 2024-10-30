using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

public class IndexModel : PageModel
{
    [BindProperty]
    public string? SearchQuery { get; set; }

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Bing News API endpoint
                string apiKey = "be1137cec3e34918ba30c5ce7495ea42"; // Replace with your Bing News API key
                string apiUrl = $"https://api.bing.microsoft.com/v7.0/news/search?q={SearchQuery}";

                using (HttpClient client = new HttpClient())
                {
                    // Add the API key to the request headers
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                    var response = await client.GetStringAsync(apiUrl);

                    // Log full response for debugging
                    _logger.LogInformation("Full response: {Response}", response);

                    // Check if response contains articles and store in TempData
                    var articles = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    if (articles != null && articles.ContainsKey("value"))
                    {
                        TempData["SearchResults"] = JsonConvert.SerializeObject(articles["value"]);
                        TempData.Keep("SearchResults"); // Ensure TempData persists across the redirect
                    }
                    else
                    {
                        _logger.LogInformation("No articles found.");
                        TempData["SearchResults"] = "No articles found.";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching articles from Bing News API.");
            TempData["SearchResults"] = "An error occurred while searching for articles. Please try again later.";
        }

        return RedirectToPage("/PublicSearchResults");
    }
}
