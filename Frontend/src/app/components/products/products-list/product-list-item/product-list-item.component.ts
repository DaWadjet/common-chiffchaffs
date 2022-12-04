import { Component, Input } from '@angular/core';
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
}
