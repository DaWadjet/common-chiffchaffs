import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable } from 'rxjs';
import { CommentDto } from '../generated/webshopApiClient';
import { CommentService } from './comment.service';
import { ProductsService } from './products.service';

@Injectable({
  providedIn: 'root',
})
export class SingleCommentResolverService
  implements Resolve<CommentDto | undefined>
{
  constructor(
    private productsService: ProductsService,
    private commentService: CommentService
  ) {}
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | CommentDto
    | Observable<CommentDto | undefined>
    | Promise<CommentDto | undefined>
    | undefined {
    const currentlyLoadedProduct = this.productsService.selectedProduct.value;
    const productId = route.params['productId'];
    const commentId = route.params['commentId'];
    if (currentlyLoadedProduct?.id === productId) {
      const comment = currentlyLoadedProduct!.comments!.find(
        (c) => c.id === commentId
      )!;
      this.commentService.commentUnderEdit.next(comment);
      return comment;
    } else {
      let fetchedComment: CommentDto | undefined;
      this.productsService.fetchProductById(productId).subscribe((p) => {
        const comment = p.comments!.find((c) => c.id === commentId)!;
        this.commentService.commentUnderEdit.next(comment);
        fetchedComment = comment;
      });
      return fetchedComment;
    }
  }
}
