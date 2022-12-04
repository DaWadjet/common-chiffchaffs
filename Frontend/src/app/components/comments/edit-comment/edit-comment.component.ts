import { Location } from '@angular/common';
import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { CommentDto } from 'src/app/generated/webshopApiClient';
import { CommentService } from './../../../services/comment.service';

@Component({
  selector: 'app-edit-comment',
  templateUrl: './edit-comment.component.html',
  styleUrls: ['./edit-comment.component.scss'],
})
export class EditCommentComponent implements OnInit, OnDestroy, AfterViewInit {
  constructor(
    private commentService: CommentService,
    private location: Location,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService
  ) {}

  commentUnderEdit?: CommentDto;
  productId!: string;
  subscription?: Subscription;
  @ViewChild('commentForm', { static: false }) commentForm!: NgForm;
  isLoading = false;

  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      this.productId = params['productId'];
    });

    this.subscription = this.commentService.commentUnderEdit.subscribe(
      (comment) => {
        if (!comment) {
          this.router.navigate(['/products']);
        }
        this.commentUnderEdit = comment;
      }
    );
  }
  ngAfterViewInit(): void {
    setTimeout(() => {
      this.commentForm.setValue({
        commentContent: this.commentUnderEdit?.content ?? '',
      });
    }, 1);
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  onSubmit(form: NgForm) {
    if (!form.valid) {
      return;
    }

    console.log(form.value, this.productId);
    this.isLoading = true;
    const content = form.value.commentContent;

    const updateCommentObs = this.commentService.updateComment(
      content,
      this.commentUnderEdit!.id!,
      this.productId
    );

    updateCommentObs.subscribe({
      next: (resData) => {
        this.toastr.success('Comment sent!');
        this.back();
        this.isLoading = false;
      },
      error: (errorMessage) => {
        console.log(errorMessage);
        this.isLoading = false;

        this.toastr.error('Please try again later', 'Failed to send comment');
      },
    });

    form.reset();
  }

  back(): void {
    this.location.back();
  }
}
