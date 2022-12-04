import { Injectable } from '@angular/core';
import { BehaviorSubject, flatMap } from 'rxjs';
import { CommentDto, WebshopApiClient } from './../generated/webshopApiClient';
import { ProductsService } from './products.service';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  constructor(
    private api: WebshopApiClient,
    private productsService: ProductsService
  ) {}

  commentUnderEdit = new BehaviorSubject<CommentDto | undefined>(undefined);

  public sendComment(content: string, productId: string) {
    return this.api
      .comment_SaveComment({ content, productId })
      .pipe(flatMap(() => this.productsService.fetchProductById(productId)));
  }

  public deleteComment(commentId: string, productId: string) {
    return this.api
      .comment_DeleteComment({ commentId })
      .pipe(flatMap(() => this.productsService.fetchProductById(productId)));
  }

  public updateComment(commentId: string, content: string, productId: string) {
    console.log('updateComment', commentId, content, productId);
    return this.api
      .comment_UpdateComment({ commentId, content })
      .pipe(flatMap(() => this.productsService.fetchProductById(productId)));
  }
}
