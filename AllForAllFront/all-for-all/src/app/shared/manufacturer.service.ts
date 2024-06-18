import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Manufacturer } from '../models/manufacturer.model';

@Injectable({
  providedIn: 'root'
})

export class ManufacturerService {
    private manufacturerUrl = 'http://localhost:5158/api/manufacturers'; 
  
    constructor(private http: HttpClient) { }
  
    getManufacturers(): Observable<Manufacturer[]> {
      return this.http.get<Manufacturer[]>(this.manufacturerUrl);
    }

    getManufacturerById(id: number): Observable<Manufacturer> {
      const url = `${this.manufacturerUrl}/${id}`;
      return this.http.get<Manufacturer>(url);
    }

    getPopularManufacturers(): Observable<Manufacturer[]> {
      const url = `${this.manufacturerUrl}/popular`;
      return this.http.get<Manufacturer[]>(url);
    }
}