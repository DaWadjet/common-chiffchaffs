import { Component, Input } from '@angular/core';
import { saveAs } from 'file-saver';
import { ToastrService } from 'ngx-toastr';
import { ProductsService } from 'src/app/services/products.service';
import { ProductDto } from '../../../../generated/webshopApiClient';

@Component({
  selector: 'app-product-list-item',
  templateUrl: './product-list-item.component.html',
})
export class ProductListItemComponent {
  constructor(
    private productsService: ProductsService,
    private toastr: ToastrService
  ) {}
  @Input() product!: ProductDto;
  @Input() shouldShowEditAndDeleteButton: boolean = false;
  @Input() shouldShowBuyButton: boolean = true;
  @Input() shouldShowDownloadButton: boolean = false;

  deleteProduct(): void {
    const obs = this.productsService.deleteProduct(this.product.id!);

    obs.subscribe({
      next: () => {
        this.toastr.success('Product deleted');
      },
      error: (err) => {
        this.toastr.error("Couldn't delete product", 'An error occurred');
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

  downloadProduct(): void {
    const obs = this.productsService.downloadProduct(
      '3d39b432-2b4b-42f8-a4f9-bde76ab79916'
    );

    obs.subscribe({
      next: (resData) => {
        let blob = new Blob([resData.data], {
          type: 'application/octet-stream',
        });
        saveAs(blob, (this.product.name ?? 'file') + '.caff');
        this.toastr.success(
          'Your file is being downloaded.',
          'Check your downloads!'
        );
      },
      error: (errorMessage) => {
        console.log(errorMessage);
        this.toastr.error(errorMessage);
      },
    });
  }
}
