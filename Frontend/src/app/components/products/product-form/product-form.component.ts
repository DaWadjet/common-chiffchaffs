import { Location } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ProductDto } from 'src/app/generated/webshopApiClient';
import { ProductsService } from './../../../services/products.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss'],
})
export class ProductFormComponent implements OnInit {
  constructor(
    private productsService: ProductsService,
    private location: Location
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

  ngOnInit(): void {}

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
      // } else {
      //   const updateTaskObs = this.taskDataService.updateTask(
      //     name,
      //     description,
      //     this.taskId!,
      //     type,
      //     this.projectId!
      //   );

      //   updateTaskObs.subscribe({
      //     next: (resData) => {
      //       this.isLoading = false;
      //       this.toastr.success('Task updated!');
      //       this.back();
      //     },
      //     error: (errorMessage) => {
      //       console.log(errorMessage);
      //       this.toastr.error(errorMessage);
      //       this.isLoading = false;
      //     },
      //   });
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
