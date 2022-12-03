import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import * as webshopApiClient from './generated/webshopApiClient';

import { environment } from 'src/environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './common/navbar/navbar.component';
import { ProductDetailsComponent } from './components/product-details/product-details.component';

export function initializeApp(oauthService: OAuthService): any {
  return async () => {
    oauthService.configure({
      clientId: 'webshop_client',
      issuer: 'https://localhost:5001',
      postLogoutRedirectUri: 'https://localhost:5001',
      redirectUri: 'http://localhost:4200',
      requireHttps: true,
      responseType: 'code',
      scope: 'openid full-access',
      useSilentRefresh: true,
      skipIssuerCheck: true,
    });
    oauthService.setupAutomaticSilentRefresh();
    return oauthService.loadDiscoveryDocumentAndTryLogin();
  };
}

@NgModule({
  declarations: [AppComponent, NavbarComponent, ProductDetailsComponent],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: ['https://localhost:5001'],
        sendAccessToken: true,
      },
    }),
  ],
  providers: [
    {
      provide: webshopApiClient.API_BASE_URL,
      useValue: environment.apiBaseUrl,
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [OAuthService],
      multi: true,
    },
    webshopApiClient.WebshopApiClient,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
