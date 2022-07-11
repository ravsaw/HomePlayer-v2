using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomePlayer_SharedModel
{
    public class Movie
    {
        public string Title { get; set; }
        public string FileName { get; set; }

        public Movie()
        {
            Title = "";
            FileName = Guid.NewGuid().ToString("n") + ".mp4";
        }

        public static void SerializeToFile(IList<Movie> list, string? filepath = null)
        {
            string jsonString = JsonSerializer.Serialize(list);
            File.WriteAllText(filepath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer", "list.json"), jsonString);
        }

        public static List<Movie> DeserializeFromFile(string? filepath = null)
        {
            try
            {
                string jsonString = File.ReadAllText(filepath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer", "list.json"));
                return JsonSerializer.Deserialize<List<Movie>>(jsonString)!;
            }
            catch (Exception)
            {
                return new();
            }
        }
    }


    public class UploadResult
    {
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public int ErrorCode { get; set; }
    }
}
