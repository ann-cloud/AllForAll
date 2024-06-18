import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { Product } from '../models/product.model';
import { ProductRequestDto } from '../models/productdto.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private productUrl = 'http://localhost:5158/api/products';
  private countriesSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  public countries$: Observable<string[]> = this.countriesSubject.asObservable();

  constructor(private http: HttpClient) {
    this.fetchCountries();
  }

  getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(this.productUrl);
  }


  getProductById(id: number): Observable<Product> {
    const url = `${this.productUrl}/${id}`;
    return this.http.get<Product>(url);
  }

  getPopularProducts(): Observable<Product[]> {
    const url = `${this.productUrl}/popular`;
    return this.http.get<Product[]>(url);
  }

  getFilteredProducts(section: string, filterValue: string): Observable<Product[]> {
    return this.http.get<Product[]>(this.productUrl).pipe(
      map(products => {
        switch (section) {
          case 'countries':
            return products.filter(product => product.country === filterValue);
          case 'categories':
            return products.filter(product => product.category.name === filterValue);
          case 'manufacturers':
            return products.filter(product => product.manufacturer.name === filterValue);
          default:
            return products;
        }
      })
    );
  }

  private fetchCountries(): void {
    this.http.get<Product[]>(this.productUrl).pipe(
      map(products => {
        const countrySet = new Set(products.map(product => product.country));
        return Array.from(countrySet);
      })
    ).subscribe({
      next: countries => {
        this.countriesSubject.next(countries);
      },
      error: error => {
        console.error('Error fetching countries in service:', error);
      }
    });
  }
  createProduct(productDto: ProductRequestDto, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('ProductName', productDto.productName);
    formData.append('Country', productDto.country);
    formData.append('CategoryId', productDto.category.toString()); 
    formData.append('ManufacturerId', productDto.manufacturer.toString());
    return this.http.post<any>(`${this.productUrl}`, formData);
  }
}
