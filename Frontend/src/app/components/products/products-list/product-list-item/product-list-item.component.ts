import { Component, Input } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';
import { ProductDto } from '../../../../generated/webshopApiClient';

@Component({
  selector: 'app-product-list-item',
  templateUrl: './product-list-item.component.html',
})
export class ProductListItemComponent {
  constructor(private productsService: ProductsService) {}
  @Input() product!: ProductDto;
  @Input() shouldShowEditAndDeleteButton: boolean = false;

  deleteProduct(): void {
    const obs = this.productsService.deleteProduct(this.product.id!);

    obs.subscribe(() => {
      console.log('Product deleted');
    });
  }
  buyProduct(): void {
    const obs = this.productsService.buyProduct(this.product.id!);

    obs.subscribe(() => {
      console.log('Product bought');
    });
  }
}
