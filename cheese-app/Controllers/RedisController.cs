using cheese_app.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace cheese_app.Controllers
{
    public class RedisController : ApiController
    {
        // GET api/redis/status
        [HttpGet]
        [Route("api/redis/status")]
        public IHttpActionResult GetStatus()
        {
            try
            {
                var isConnected = RedisService.IsConnected();
                var status = RedisService.GetConnectionStatus();

                return Ok(new
                {
                    connected = isConnected,
                    status = status,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Redis connection error: {ex.Message}"));
            }
        }

        // GET api/redis/{key}
        [HttpGet]
        [Route("api/redis/{key}")]
        public IHttpActionResult Get(string key)
        {
            try
            {
                var db = RedisService.GetDatabase();
                var value = db.StringGet(key);

                if (value.IsNullOrEmpty)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    key = key,
                    value = value.ToString()
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error getting key '{key}': {ex.Message}"));
            }
        }

        // POST api/redis
        [HttpPost]
        [Route("api/redis")]
        public IHttpActionResult Post([FromBody] RedisKeyValue model)
        {
            if (model == null || string.IsNullOrEmpty(model.Key))
            {
                return BadRequest("Key is required");
            }

            try
            {
                var db = RedisService.GetDatabase();
                var expiry = model.ExpirySeconds.HasValue
                    ? TimeSpan.FromSeconds(model.ExpirySeconds.Value)
                    : (TimeSpan?)null;

                db.StringSet(model.Key, model.Value ?? string.Empty, expiry);

                return Ok(new
                {
                    message = "Key set successfully",
                    key = model.Key,
                    value = model.Value,
                    expirySeconds = model.ExpirySeconds
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error setting key '{model.Key}': {ex.Message}"));
            }
        }

        // DELETE api/redis/{key}
        [HttpDelete]
        [Route("api/redis/{key}")]
        public IHttpActionResult Delete(string key)
        {
            try
            {
                var db = RedisService.GetDatabase();
                var deleted = db.KeyDelete(key);

                if (!deleted)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    message = "Key deleted successfully",
                    key = key
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error deleting key '{key}': {ex.Message}"));
            }
        }

        // GET api/redis/keys/{pattern}
        [HttpGet]
        [Route("api/redis/keys/{pattern?}")]
        public IHttpActionResult GetKeys(string pattern = "*")
        {
            try
            {
                var server = RedisService.GetServer();
                var keys = server.Keys(pattern: pattern, pageSize: 100)
                    .Take(100)
                    .Select(k => k.ToString())
                    .ToList();

                return Ok(new
                {
                    pattern = pattern,
                    count = keys.Count,
                    keys = keys
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error getting keys: {ex.Message}"));
            }
        }
    }

    public class RedisKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int? ExpirySeconds { get; set; }
    }
}
