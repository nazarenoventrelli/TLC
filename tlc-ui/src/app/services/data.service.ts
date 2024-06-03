import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private endpointUrl = 'https://ht5b638b8l.execute-api.us-east-1.amazonaws.com/dev/search';

  constructor(private http: HttpClient) { }

  getTaxiData(queryRequest: any): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    return this.http.post(this.endpointUrl, queryRequest, httpOptions);
  }
}
