import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { CommentService } from './comment.service';
import { ProductsService } from './products.service';

@Injectable({
  providedIn: 'root',
})
export class LogoutService {
  constructor(
    private oauthService: OAuthService,
    private productsService: ProductsService,
    private commentService: CommentService
  ) {}

  logout() {
    this.productsService.clearProducts();
    this.commentService.commentUnderEdit.next(undefined);
    this.oauthService.logOut();
  }
}
