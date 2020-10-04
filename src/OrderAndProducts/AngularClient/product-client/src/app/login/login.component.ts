import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { LoginService } from '../services/login.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  userName: string;
  userPassword: string;
  errorMessage: boolean;
  return: string = '';
  constructor(private loginService: LoginService,
              private cookieService: CookieService,
              private router: Router,
              private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.queryParams
    .subscribe(params => this.return = params['return'] || '/products');
  }

  submitLogin()
  {
    const dateNow = new Date();
      dateNow.setMinutes(dateNow.getMinutes() + 15);
      this.loginService.loginUser(this.userName, this.userPassword).subscribe(result => {
      this.cookieService.set("authToken", result.jwtToken, dateNow);
      this.cookieService.set("refreshToken", result.refreshToken);
      this.cookieService.set("userId", result.userId);
      this.cookieService.set("userName", result.userName);
      this.cookieService.set("userRole", result.role);
      this.router.navigateByUrl(this.return);
    },error => {
      this.errorMessage = true;
    }
    )
  }
}
