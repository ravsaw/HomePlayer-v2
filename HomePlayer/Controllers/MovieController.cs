using HomePlayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace HomePlayer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {

        private readonly ILogger<MovieController> _logger;

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Movie> Get()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer");
            var movies = new List<Movie>();
            var files = Directory.GetFiles(path);
            foreach (var fullpath in files)
            {
                var fileName = Path.GetFileName(fullpath);
                var movie = new Movie() { Title = fileName };
                movies.Add(movie);
            }
            return movies;
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public FileResult PlayVideo(string id)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer");
            var filePath = Path.Combine(path, id);
            return PhysicalFile(filePath, contentType: "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpPost, DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {

            try
            {
                var file = Request.Form.Files[0];
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString("n").Substring(0, 10) + ".mp4";
                    string fullPath = Path.Combine(path, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                return Ok("Upload Successful.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Upload Failed: " + ex.Message);
            }
        }
    }


}