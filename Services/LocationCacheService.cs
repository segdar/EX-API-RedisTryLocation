using EX_API_RedisTryLocation.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace EX_API_RedisTryLocation.Services
{
    public class LocationCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ContextDBServices _context;

        public LocationCacheService(
            IDistributedCache cache,
            ContextDBServices context)
        {
            _cache = cache;
            _context = context;
        }


        public async Task<List<NearLocation>> GetLocationsAsync(NearLocationRequest request)
        {
            string cacheKey = $"locations:{request.Latitude}:{request.Longitude}:{request.Radius}";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<List<NearLocation>>(cachedData)!;
            }

       
            var locations  = await _context.locations
                 .FromSqlInterpolated($"EXEC dbo.SP_GetNearbyStores @Latitude = {request.Latitude}, @Longitude = {request.Longitude}, @RadiusMeters = {request.Radius}")
                 .AsNoTracking()
                 .ToListAsync();


            var jsonData = JsonSerializer.Serialize(locations);

            await _cache.SetStringAsync(
                "locations",
                jsonData,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return locations;
        }

        public async Task RemoveLocationsCacheAsync()
        {
            await _cache.RemoveAsync("locations");
        }
    }
}
