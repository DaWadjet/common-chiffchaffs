import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
//import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root',
})
export class LogoutService {
  //constructor(private cookieService: CookieService, private router: Router) {}

  logout() {
    /*sessionStorage.clear();
    this.cookieService.deleteAll();
    this.router.navigate(['/']);*/
  }
}
