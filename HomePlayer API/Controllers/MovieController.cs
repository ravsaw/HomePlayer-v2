using HomePlayer_SharedModel;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            return Movie.DeserializeFromFile();
        }

        [HttpGet]
        [Route("{id}")]
        public FileResult PlayVideo(string id)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer", id);
            return PhysicalFile(path, contentType: "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("bak")]
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


        [HttpPost]
        public async Task<ActionResult<UploadResult>> PostFile([FromForm] IFormFile file, [FromForm] Movie movie)
        {
            long maxFileSize = 10737418240;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            var uploadResult = new UploadResult();
            uploadResult.FileName = file.FileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(file.FileName);

            if (file.Length == 0)
            {
                Console.WriteLine(trustedFileNameForDisplay + " length is 0 (Err: 1)");
                uploadResult.ErrorCode = 1;
            }
            else if (file.Length > maxFileSize)
            {
                Console.WriteLine(trustedFileNameForDisplay + " of " + file.Length + " bytes is larger than the limit of " + maxFileSize + " bytes (Err: 2)");
                uploadResult.ErrorCode = 2;
            }
            else
            {
                try
                {
                    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer", file.FileName);

                    await using FileStream fs = new(path, FileMode.Create);
                    await file.CopyToAsync(fs);

                    Console.WriteLine(trustedFileNameForDisplay + " saved at " + path);
                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = file.FileName;
                    var movies = Movie.DeserializeFromFile();
                    movies.Add(movie);
                    Movie.SerializeToFile(movies);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(trustedFileNameForDisplay + " error on upload (Err: 3): " + ex.Message);
                    uploadResult.ErrorCode = 3;
                }
            }

            return new CreatedResult(resourcePath, uploadResult);
        }
    }
}