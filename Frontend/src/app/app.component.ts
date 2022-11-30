import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { WebshopApiClient } from './generated/webshopApiClient';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'Frontend';

  constructor(
    private oauthService: OAuthService,
    private webshopClient: WebshopApiClient
  ) {}

  ngOnInit(): void {
    if (!this.oauthService.hasValidAccessToken()) {
      this.oauthService.initCodeFlow();
    }
  }

  logout() {
    this.oauthService.logOut();
  }

  getNormal() {
    this.webshopClient
      .weatherForecast_Get()
      .subscribe(() => console.log('Normal get: OK!'));
  }

  getAuthorized() {
    this.webshopClient
      .weatherForecast_GetAuthorized(null)
      .subscribe(() => console.log('Authorize get: OK!'));
  }
}