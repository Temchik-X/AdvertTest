using Microsoft.AspNetCore.Mvc;
using AdvertTest.Services;
using System.Collections.Generic;

namespace Advert.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisingController : ControllerBase
    {
        private readonly AdvertisingService _advertisingService;

        public AdvertisingController(AdvertisingService advertisingService)
        {
            _advertisingService = advertisingService;
        }

        [HttpPost("load")]
        public IActionResult LoadPlatforms([FromBody] string filePath)
        {
            try
            {
                _advertisingService.LoadFromFile(filePath);
                return Ok("Данные успешно загружены.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        [HttpGet("find/{location}")]
        public IActionResult FindPlatforms(string location)
        {
            try
            {
                // Декодируем URL-закодированную строку
                var decodedLocation = Uri.UnescapeDataString(location);
                var platforms = _advertisingService.FindPlatformsForLocation(decodedLocation);
                return Ok(platforms);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при поиске площадок: {ex.Message}");
            }
        }
    }
}
