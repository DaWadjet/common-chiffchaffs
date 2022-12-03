import { Component, OnInit } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  constructor(private oauthService: OAuthService) {}

  ngOnInit(): void {
    if (!this.oauthService.hasValidAccessToken()) {
      this.oauthService.initCodeFlow();
    }
  }
}
