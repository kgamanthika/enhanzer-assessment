import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
    PurchaseBillRequest
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class PurchaseBillService {
  private readonly http = inject(HttpClient);

private readonly apiUrl = 'http://localhost:5240/api';
  create(
    request: PurchaseBillRequest
  ): Observable<unknown> {
    return this.http.post(
      `${this.apiUrl}/PurchaseBills`,
      request
    );
  }
}