import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Feedback } from '../models/feedback.model';

@Injectable({
  providedIn: 'root'
})

export class FeedbackService {
    private feedbackUrl = 'http://localhost:5158/api/Feedback'; 
  
    constructor(private http: HttpClient) { }
  
    getFeedbacks(): Observable<Feedback[]> {
      return this.http.get<Feedback[]>(this.feedbackUrl);
    }

    postFeedback(feedbackData: any): Observable<number> {
      const token = localStorage.getItem('token');
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });
      return this.http.post<number>(this.feedbackUrl, feedbackData, { headers: headers });
    }

}