import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { ProductDto } from 'src/app/generated/webshopApiClient';
import { CommentService } from './../../../services/comment.service';
import { ProductsService } from './../../../services/products.service';
import { RoleService } from './../../../services/role.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss'],
})
export class ProductDetailsComponent implements OnInit, OnDestroy {
  constructor(
    private productsService: ProductsService,
    private roleService: RoleService,
    private location: Location,

    private commentService: CommentService,
    private toastr: ToastrService
  ) {}

  product!: ProductDto;
  productSub!: Subscription;

  @ViewChild('commentForm', { static: false }) commentForm!: NgForm;

  ngOnInit(): void {
    this.productSub = this.productsService.selectedProduct.subscribe(
      (product) => {
        this.product = product!;
      }
    );

    this.product = this.productsService.selectedProduct.value!;
  }
  ngOnDestroy(): void {
    this.productSub.unsubscribe();
  }

  onCommentSubmit(form: NgForm) {
    if (!form.valid) {
      return;
    }

    const content = form.value.commentContent;

    const sendCommentObs = this.commentService.sendComment(
      content,
      this.product.id!
    );

    sendCommentObs.subscribe({
      next: (resData) => {
        this.toastr.success('Comment added!');
      },
      error: (errorMessage) => {
        console.log(errorMessage);
        this.toastr.error('Please try again later', 'An error occured!');
      },
    });

    form.reset();
  }

  get isAdmin() {
    return this.roleService.isAdmin;
  }

  back(): void {
    this.location.back();
  }

  deleteProduct(): void {
    const obs = this.productsService.deleteProduct(this.product.id!);

    obs.subscribe({
      next: (resData) => {
        this.toastr.success('Product deleted successfully!');
        this.location.back();
      },
      error: (errorMessage) => {
        this.toastr.error('Please try again later', 'An error occured!');
      },
    });
  }

  deleteComment(commentId: string): void {
    const obs = this.commentService.deleteComment(commentId, this.product.id!);

    obs.subscribe({
      next: (resData) => {
        this.toastr.success('Comment deleted successfully!');
      },
      error: (errorMessage) => {
        this.toastr.error('Please try again later', 'An error occured!');
      },
    });
  }

  buyProduct(): void {
    const obs = this.productsService.buyProduct(this.product.id!);

    obs.subscribe({
      next: () => {
        this.toastr.success('Product bought');
      },
      error: (err) => {
        this.toastr.error("Couldn't buy product", 'An error occurred');
      },
    });
  }
}
