using EX_API_RedisTryLocation.Models;
using EX_API_RedisTryLocation.Services;
using Microsoft.AspNetCore.Mvc;


namespace EX_API_RedisTryLocation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NearLocationController : ControllerBase
    {
        private readonly LocationCacheService _locationCacheService;
        public NearLocationController(LocationCacheService locationCacheService)
        {
            _locationCacheService = locationCacheService;
        }

        [HttpPost(Name = "NearLocation")]
        public async Task<IEnumerable<NearLocation>> Get([FromBody] NearLocationRequest request)
        {
            var results = await _locationCacheService.GetLocationsAsync(request);
            return results;
        }
    }
}
