using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace Shotsal.Logic.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, TimeSpan expiration);
        Task<byte[]> GetImageAsync(string key);
    }

    public class ImageService(IConnectionMultiplexer redis) : IImageService
    {
        public async Task<string> SaveImageAsync(IFormFile imageFile, TimeSpan expiration)
        {
            var db = redis.GetDatabase();
            var key = Guid.NewGuid().ToString();

            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            await db.StringSetAsync(key, imageBytes, expiration);
            return key;
        }

        public async Task<byte[]> GetImageAsync(string key)
        {
            var db = redis.GetDatabase();
            return (await db.StringGetAsync(key))!;
        }
    }
}
