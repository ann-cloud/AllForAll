import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { Register } from '../models/register.model';
import { Login } from '../models/login.model';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private userUrl = 'http://localhost:5158/api/users';

  constructor(private http: HttpClient) {
  }

  getUserById(id: number): Observable<User> {
    const url = `${this.userUrl}/GetUserByIdAsync/${id}`;
    return this.http.get<User>(url);
  }
  getCheckToken(): Observable<User> {
    const token = localStorage.getItem('token');
    const url = `${this.userUrl}/CheckToken`;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
 
    return this.http.get<User>(url, { headers: headers });
  }
  logout() {
    localStorage.removeItem('token');
  }

  registerUser(user: Register): Observable<any> {
    const url = `${this.userUrl}/RegisterUser`;
    const formData = new FormData();
    formData.append('Username', user.Username);
    formData.append('Email', user.Email);
    formData.append('Password', user.Password);
    return this.http.post<string>(url, formData);
  }
  login(user: Login): Observable<any> {
    const url = `${this.userUrl}/LoginUser`;
    const formData = new FormData();
    formData.append('Email', user.Email);
    formData.append('Password', user.Password);
    return this.http.post<string>(url, formData);
  }
}
