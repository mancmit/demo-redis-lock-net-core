using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet;

namespace Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LockController : ControllerBase
    {
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly IDistributedCache _cache;
        private readonly string _keyCache = "resource";
        private readonly string _keyLock = "lock:resource";
        private readonly TimeSpan _expiryLock = TimeSpan.FromSeconds(30);

        public LockController(
            IDistributedLockFactory distributedLockFactory,
            IDistributedCache cache)
        {
            _distributedLockFactory = distributedLockFactory;
            _cache = cache;
        }

        [HttpGet("set-resource-with-giving-up")]
        public async Task<IActionResult> SetResourceWithGivingUp([FromQuery] string value)
        {
            await using (var redLock = await _distributedLockFactory.CreateLockAsync(_keyLock, _expiryLock)) // there are also non async Create() methods
            {
                // make sure we got the lock
                if (redLock.IsAcquired)
                {
                    await SetResource(value);

                    // Lock acquired, perform your operation
                    return Ok("Lock acquired, processing resource.");
                }
            }

            return Conflict("Failed to acquire lock on the resource.");
        }

        [HttpGet("set-resource-with-retry")]
        public async Task<IActionResult> SetResourceWithRetry([FromQuery] string value)
        {
            var wait = TimeSpan.FromSeconds(10);   // Maximum time to wait for lock acquisition
            var retry = TimeSpan.FromSeconds(1);   // Retry interval if lock is not acquired

            using (var redLock = await _distributedLockFactory.CreateLockAsync(_keyLock, _expiryLock, wait, retry))
            {
                if (redLock.IsAcquired)
                {
                    await SetResource(value);

                    // Lock acquired, perform your operation
                    return Ok("Lock acquired, processing resource.");
                }
            }

            // Failed to acquire the lock
            return Conflict("Failed to acquire lock on the resource.");
        }

        [HttpGet("get-resource")]
        public async Task<IActionResult> GetResource()
        {
            var data = await _cache.GetStringAsync(_keyCache);

            return Ok(data ?? "NOT YET SET");
        }

        private async Task SetResource(string value)
        {
            await _cache.SetStringAsync(
                _keyCache,
                value,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                }
            );

            await Task.Delay(10000);
        }
    }
}
