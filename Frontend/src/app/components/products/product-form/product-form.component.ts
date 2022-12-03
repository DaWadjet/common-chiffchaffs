import { Location } from '@angular/common';
import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription } from 'rxjs';
import { ProductDto } from 'src/app/generated/webshopApiClient';
import { ProductsService } from './../../../services/products.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss'],
})
export class ProductFormComponent implements OnInit, OnDestroy, AfterViewInit {
  constructor(
    private productsService: ProductsService,
    private location: Location,
    private route: ActivatedRoute
  ) {}

  productUnderEdit?: ProductDto;
  subscription?: Subscription;
  caffFileToUpload: File | null = null;
  productId?: string;
  isLoading = false;
  @ViewChild('productForm', { static: false }) productForm!: NgForm;

  get isNew() {
    return this.productId === undefined;
  }

  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      this.productId = params['productId'];
    });
    if (!this.isNew) {
      this.subscription = this.productsService.selectedProduct.subscribe(
        (task) => {
          this.productUnderEdit = task;
        }
      );
    }
  }

  ngAfterViewInit(): void {
    if (!this.isNew) {
      setTimeout(() => {
        this.productForm.setValue({
          productName: this.productUnderEdit!.name,
          description: this.productUnderEdit!.description,
          price: this.productUnderEdit!.price,
        });
      }, 1);
    }
  }
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  onSubmit(form: NgForm) {
    console.log(form.value);
    if (!form.valid) {
      return;
    }

    const name = form.value.productName;
    const description = form.value.description;
    const price = form.value.price;
    const caffFile = form.value.caffFile;

    this.isLoading = true;

    if (this.isNew) {
      const createProductObs = this.productsService.createProduct(
        name,
        description,
        price,
        {
          data: this.caffFileToUpload as Blob,
          fileName: caffFile,
        }
      );

      createProductObs.subscribe({
        next: (resData) => {
          this.isLoading = false;
          this.back();
        },
        error: (errorMessage) => {
          console.log(errorMessage);
          this.isLoading = false;
        },
      });
    } else {
      const updateTaskObs = this.productsService.updateProduct(
        name,
        description,
        price,
        this.productUnderEdit!.id!
      );

      updateTaskObs.subscribe({
        next: (resData) => {
          this.isLoading = false;
          this.back();
        },
        error: (errorMessage) => {
          console.log(errorMessage);
          this.isLoading = false;
        },
      });
    }

    form.reset();
  }

  onFileChange(event: any) {
    let fileList: FileList = event.target.files;
    if (fileList.length > 0) {
      let file: File = fileList[0];
      this.caffFileToUpload = file;
      console.log(file);
    }
  }

  back(): void {
    this.location.back();
  }
}
