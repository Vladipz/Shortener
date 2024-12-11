import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UrlData } from '../interfaces/url-data.interface';

@Injectable({
  providedIn: 'root',
})
export class UrlService {
  baseUrl = `${environment.baseApiUrl}api/`;
  constructor(private http: HttpClient) {}

  // Method to get all URLs
  getUrls(): Observable<UrlData[]> {
    return this.http.get<UrlData[]>(`${this.baseUrl}ShortUrl`);
  }

  // Method to create a new short URL
  createUrl(longUrl: string): Observable<UrlData> {
    return this.http.post<UrlData>(`${this.baseUrl}ShortUrl/create`, {
      longUrl,
    });
  }
}
