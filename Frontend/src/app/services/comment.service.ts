import { Injectable } from '@angular/core';
import { flatMap } from 'rxjs';
import { WebshopApiClient } from './../generated/webshopApiClient';
import { ProductsService } from './products.service';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  constructor(
    private api: WebshopApiClient,
    private productsService: ProductsService
  ) {}

  public sendComment(content: string, productId: string) {
    return this.api
      .comment_SaveComment({ content, productId })
      .pipe(flatMap(() => this.productsService.fetchProductById(productId)));
  }
}
