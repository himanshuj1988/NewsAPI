using NewsAPI;
using NewsAPI.Repository;
using Newtonsoft.Json;

namespace NewsAPITest
{
    public class UnitTest1
    {
        [Fact]
        public async Task<bool> CheckNewsAPI()
        {
            NewsRepository newsRep = new NewsRepository();
            List<NewsStory> stories = new List<NewsStory>();

            var response = await newsRep.BestStoriesAsync();
            if (response.IsSuccessStatusCode)
            {
                var storiesResponse = response.Content.ReadAsStringAsync().Result;
                var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);
                var res = bestIds.Count > 0 ? (bestIds.Contains(37825292) ? true : false) : false;
                Assert.True(res);
                return res;
            }

            else
            {
                Assert.Fail("Not able to get response from API");
                return false;
            }
        }
            

        
    }
}