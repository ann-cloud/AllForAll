import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})

export class CategoryService {
  private categoriesUrl = 'http://localhost:5158/api/categories'; 

  constructor(private http: HttpClient) { }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.categoriesUrl);
  }

  getCategoryById(id: number): Observable<Category> {
    const url = `${this.categoriesUrl}/${id}`;
    return this.http.get<Category>(url);
  }

  getPopularCategories(): Observable<Category[]> {
    const url = `${this.categoriesUrl}/popular`;
    return this.http.get<Category[]>(url);
  }
}