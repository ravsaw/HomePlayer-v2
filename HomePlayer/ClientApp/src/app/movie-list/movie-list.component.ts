import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-movie-list',
  templateUrl: './movie-list.component.html',
  styleUrls: ['./movie-list.component.css']
})

export class MovieListComponent {
  public movies: Movie[] = [];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.getMovieList(http, baseUrl);
  }

  private getMovieList(http: HttpClient, baseUrl: string) {
    http
      .get<Movie[]>(baseUrl + 'movie')
      .subscribe(result => {
        this.movies = result;
      }, error => console.error(error));
  }

  public playMovie(title: string) {

  }
}

interface Movie {
  title: string;
}
