using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using NewsAPI;
using NewsAPI.Repository;
using Newtonsoft.Json;
using Xunit.Sdk;

namespace NewsAPITest
{
    public class NewsAPI
    {
        MemoryCache cache;

        public NewsAPI()
        {
            cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task<bool> CheckNewsAPI()
        {

            NewsRepository newsRep = new NewsRepository();
            List<NewsStory> stories = new List<NewsStory>();
            var response = await cache.GetOrCreateAsync<List<int>>("",
            async cacheEntry =>
            {
                var response = await newsRep.BestStoriesAsync();
                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);
                return bestIds;
            });

            if (response.Count > 0)
            {
                var res = response.Count() > 0 ? (response.Contains(37825292) ? true : false) : false;
                Assert.True(res);
                return res;
            }
            else
            {
                Assert.Fail("Not able to get response from API");
                return false;
            }
        }


        [Fact]
        public async Task<bool> GetStoryAsync()
        {

            NewsRepository newsRep = new NewsRepository();
            List<NewsStory> stories = new List<NewsStory>();

            var response = await newsRep.BestStoriesAsync();
            if (response.IsSuccessStatusCode)
            {

                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);
                var res = await cache.GetOrCreateAsync<NewsStory>(bestIds[0],
                async cacheEntry =>
                {
                    NewsStory story = new NewsStory();
                    var response = await newsRep.GetStoryByIdAsync(bestIds[0]);
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = response.Content.ReadAsStringAsync().Result;
                        story = JsonConvert.DeserializeObject<NewsStory>(storyResponse);
                    }
                    return story;
                });
                Assert.Equal("Firefox tooltip bug fixed after 22 years", res.Title);
                return res.Title == "Firefox tooltip bug fixed after 22 years" ? true : false;

            }
            else
            {
                Assert.Fail("Not able to get response from API");
                return false;
            }


        }
    }



}