using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NewsAPI.Repository;
using NewsAPI.Repository.Interfaces;
using Newtonsoft.Json;

namespace NewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {

        private readonly ILogger<NewsController> _logger;

        private IMemoryCache _cache;

        private readonly INewsRepository _repo;

        public NewsController(IMemoryCache cache, INewsRepository repository)
        {
            _cache = cache;
            _repo = repository;
        }

        /// <summary>
        /// This method we are using to pull all the information from the API server based on searchfilter
        /// Created By : Himanshu Joshi
        /// Created On : 08/10/2023
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>

        [HttpGet(Name = "News")]
        public async Task<List<NewsStory>> GetNews(string? searchTerm)
        {
            List<NewsStory> stories = new List<NewsStory>();

            var response = await _repo.BestStoriesAsync();
            if (response.IsSuccessStatusCode)
            {
                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);

                var tasks = bestIds.Select(GetStoryAsync);
                stories = (await Task.WhenAll(tasks)).ToList();

                if (!String.IsNullOrEmpty(searchTerm))
                {
                    var search = searchTerm.ToLower();
                    stories = stories.Where(s =>
                                       s.Title.ToLower().IndexOf(search) > -1 || s.By.ToLower().IndexOf(search) > -1)
                                       .ToList();
                }
            }
            return stories;
        }



        /// <summary>
        /// Once we get the StoryID then we are using this method to pull all the description of Stories
        /// Created By : Himanshu Joshi
        /// Created On : 08/10/2023
        /// </summary>
        /// <param name="storyId"></param>
        /// <returns></returns>

        [HttpGet(Name = "GetStory")]
        private async Task<NewsStory> GetStoryAsync(int storyId)
        {
            return await _cache.GetOrCreateAsync<NewsStory>(storyId,
                async cacheEntry =>
                {
                    NewsStory story = new NewsStory();

                    var response = await _repo.GetStoryByIdAsync(storyId);
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonConvert.DeserializeObject<NewsStory>(storyResponse);
                    }

                    return story;
                });
        }

    }
}