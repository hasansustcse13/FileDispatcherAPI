using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Core.API.DTOs;
using System.Linq;

namespace Core.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _imageDirectory;

        public FilesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadImages(RequestDownload request)
        {
            var urlAndNames = new Dictionary<string, string>();
            var semaphore = new SemaphoreSlim(request.MaxDownloadAtOnce);
            var errors = new List<string>();

            var tasks = request.ImageUrls.Select(async url =>
            {
                await semaphore.WaitAsync();

                try
                {
                    var response = await _httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var fileName = $"{Guid.NewGuid()}";
                        var filePath = Path.Combine(_imageDirectory, fileName);
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using var fileStream = new FileStream(filePath, FileMode.Create);
                            await stream.CopyToAsync(fileStream);
                        }
                        urlAndNames[url] = fileName;
                    }
                    else
                    {
                        errors.Add($"Failed to download image from {url}: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to download image from {url}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            var success = urlAndNames.Count > 0;
            var message = string.Join(Environment.NewLine, errors);
            var result = new ResponseDownload
            {
                Success = success,
                Message = message,
                UrlAndNames = urlAndNames
            };

            return Ok(result);
        }

        [HttpGet("get-image-by-name/{imageName}")]
        public async Task<IActionResult> GetImageByName(string imageName)
        {
            var filePath = Path.Combine(_imageDirectory, imageName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return Ok(Convert.ToBase64String(bytes));
        }
    }
}


// Use this data to test the POST method. From the response you can call the GET method
/*
{
    "imageUrls": [
        "https://images.unsplash.com/profile-1449546653256-0faea3006d34", 
        "https://images.unsplash.com/photo-1449614115178-cb924f730780",
        "https://images.unsplash.com/profile-1450003783594-db47c765cea3",
        "https://images.unsplash.com/profile-1444840959767-6286d046f7f2",
        "https://images.unsplash.com/photo-1454625233598-f29d597eea1e",
        "https://images.unsplash.com/profile-1453284965521-5bd2363623de",
        "https://images.unsplash.com/profile-1544707963613-16baf868f301",
        "https://images.unsplash.com/photo-1540538581514-1d465aaad58c"
    ],
    "maxDownloadAtOnce": 3
}
*/
