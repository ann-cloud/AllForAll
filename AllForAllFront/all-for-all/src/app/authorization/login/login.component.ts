import { Component, Renderer2 } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Login } from '../../models/login.model';
import { UserService } from '../../shared/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
@Injectable({
  providedIn: 'root'
})
export class LoginComponent {
  email: string;
  password: string;
  errorIncorrectEmailPassword: boolean = false;
  errorEmailFormat: boolean = false;
  errorShortPassword: boolean = false;

 constructor(private render: Renderer2, private http: HttpClient,private router: Router,private userService: UserService){}
 ngOnInit(): void{
  const script = this.render.createElement('script');
  script.src = "https://accounts.google.com/gsi/client"
  script.onload = () => {
     console.log("load")
  }
  script.oneerror = (error:any) => {
    console.error("no load", error)
 }
 this.render.appendChild(document.body,script);
 }

 ngAfterViewInit():void {
  (window as any)['hendelOauthResponse'] = (response: unknown) => {
    const responseObj = response as any;
    if(responseObj && responseObj.credential){
      console.log(responseObj);
      this.sendResponseToServer(responseObj.credential)
    }
  }
 }
 private sendResponseToServer(credential: any): void {
  const credentialString: string = JSON.stringify(credential);
  const url = 'http://localhost:5158/api/users/LoginGoogle';
  const headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  const requestBody = credentialString;
  this.http.post(url, requestBody, { headers }).subscribe(
    (response: any) => {
      console.log('Response from server:', response);
      localStorage.setItem('token', response.token);
      this.router.navigate(['/home']);
    },
    (error) => {
      console.error('Error sending data to server:', error);
    }
  );
  
}
login(): void {
  const emailPattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  this.errorEmailFormat = false;
  this.errorShortPassword = false;

  if (!emailPattern.test(this.email) || !this.email) {
    this.errorEmailFormat = true;
  }
  if (!this.password) {
    this.errorShortPassword = true;
  }
  if (this.errorEmailFormat || this.errorShortPassword) {
    return;
  }
   const user: Login = {
    Email: this.email,
    Password: this.password
  };

  this.userService.login(user).subscribe(
    (response: any) => {
      console.log('Registration successful:', response);
      if(response.token === null){
        this.errorIncorrectEmailPassword = true;
      }
      else{
        localStorage.setItem('token', response.token);
        this.router.navigate(['/home']);
      }
    },
    (error) => {
      console.error('Error during registration:', error);
    }
  );
}


}
