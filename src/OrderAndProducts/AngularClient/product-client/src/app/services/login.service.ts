import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService} from 'ngx-cookie-service';
@Injectable({
  providedIn: 'root'
})
export class LoginService{
  constructor(private http: HttpClient, private cookieService: CookieService) { }

  loginUser(username, password){
    const url = "https://localhost:5103/api/account/authenticate";
    return this.http.post<any>(url, {Password: password, Username: username});
  }

  logoutUser(){
    this.cookieService.delete("authToken");
    this.cookieService.delete("refreshToken");
    this.cookieService.delete("userId");
    this.cookieService.delete("userName");
    this.cookieService.delete("userRole");
  }

  isLoggedIn(){
      return (this.cookieService.get('authToken') && this.cookieService.get('userId'));
  }

  getAuthToken(){
    return this.cookieService.get('authToken'); 
  }

  getUserRole(){
    return this.cookieService.get('userRole'); 
  }
}
