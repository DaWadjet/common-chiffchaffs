import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import * as webshopApiClient from './generated/webshopApiClient';
import { MatPaginatorModule } from '@angular/material/paginator';

import { FormsModule } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoaderComponent } from './components/common/loader/loader.component';
import { EditProductPageComponent } from './components/products/edit-product-page/edit-product-page.component';
import { NewProductPageComponent } from './components/products/new-product-page/new-product-page.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { ProductsHomeComponent } from './components/products/products-home/products-home.component';
import { NgbModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ProductFormComponent } from './components/products/product-form/product-form.component';
import { ProductListItemComponent } from './components/products/products-list/product-list-item/product-list-item.component';
import { ProductsListComponent } from './components/products/products-list/products-list.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

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
    EditProductPageComponent,
    NewProductPageComponent,
    ProductFormComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    NgbPaginationModule,
    MatPaginatorModule,

    AppRoutingModule,
    OAuthModule.forRoot({
      resourceServer: {
        allowedUrls: ['https://localhost:5001'],
        sendAccessToken: true,
      },
    }),
    NgbModule,
    BrowserAnimationsModule,
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
