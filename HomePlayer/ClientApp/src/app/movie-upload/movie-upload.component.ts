import { Component } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http'

@Component({
  selector: 'app-movie-upload',
  templateUrl: './movie-upload.component.html',
  styleUrls: ['./movie-upload.component.css']
})

export class MovieUploadComponent {

  public progress: number = 0;
  public message: string = "";
  public stat: Status = Status.prep;

  constructor(private http: HttpClient) { }

  checkStatus(value: keyof typeof Status) {
    return Status[value] == this.stat;
  }

  upload(file: File | null) {

    if (!file)
      return;

    const formData = new FormData();
    formData.append(file.name, file);

    const uploadReq = new HttpRequest('POST', `movie`, formData, {
      reportProgress: true,
    });

    this.stat = Status.working;

    this.http.request(uploadReq)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress && event.total)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response && event.statusText) {
          this.message = `${event.statusText}`;
          this.stat = Status.done;
        }
      });
  }
}

enum Status {
  prep,
  working,
  done
}
