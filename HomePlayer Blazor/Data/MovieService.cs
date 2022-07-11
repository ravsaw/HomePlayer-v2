using HomePlayer_SharedModel;

namespace HomePlayer_Blazor.Data
{
    public class MovieService
    {
        Movie? SelectedMovie { get; set; }

        public Task<List<Movie>> GetMoviesAsync()
        {
            return Task.Run(GetMovies);
        }

        private static List<Movie> GetMovies()
        {
            /*
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "HomePlayer");
            var movies = new List<Movie>();
            var files = Directory.GetFiles(path);
            foreach (var fullpath in files)
            {
                var fileName = Path.GetFileName(fullpath);
                var movie = new Movie() { Title = fileName, FileName = fileName };
                movies.Add(movie);
            }
            */
            return Movie.DeserializeFromFile();
        }

        public void SelectMovie(Movie movie)
        {
            SelectedMovie = movie;
        }

        public Task<Movie?> GetSelectedMovieAsync()
        {
            return Task.FromResult(SelectedMovie);
        }
    }
}
