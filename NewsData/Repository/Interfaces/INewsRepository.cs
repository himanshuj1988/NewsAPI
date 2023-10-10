namespace NewsAPI.Repository.Interfaces
{
    public interface INewsRepository
    {
        Task<HttpResponseMessage> BestStoriesAsync();
        Task<HttpResponseMessage> GetStoryByIdAsync(int id);
    }
}
