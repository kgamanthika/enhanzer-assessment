import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Location } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private readonly http = inject(HttpClient);

private readonly apiUrl = 'http://localhost:5240/api';
  getLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(
      `${this.apiUrl}/Locations`
    );
  }
}