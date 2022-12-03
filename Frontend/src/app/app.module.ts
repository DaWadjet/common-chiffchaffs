import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import * as webshopApiClient from './generated/webshopApiClient';

import { FormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoaderComponent } from './components/common/loader/loader.component';
import { ProductsListComponent } from './components/products-list/products-list.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { ProductListItemComponent } from './components/products-list/product-list-item/product-list-item.component';
import { ProductsHomeComponent } from './components/products-home/products-home/products-home.component';
import { NgbModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

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
  declarations: [
    AppComponent,
    ProductDetailsComponent,
    ProductsListComponent,
    LoaderComponent,
    ProductListItemComponent,
    ProductsHomeComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    NgbPaginationModule,

    AppRoutingModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: ['https://localhost:5001'],
        sendAccessToken: true,
      },
    }),
    NgbModule,
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
