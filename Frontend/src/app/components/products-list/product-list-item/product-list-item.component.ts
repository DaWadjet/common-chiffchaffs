import { Component, Input} from '@angular/core';
import { ProductDto, WebshopApiClient } from '../../../generated/webshopApiClient';

@Component({
  selector: 'app-product-list-item',
  templateUrl: './product-list-item.component.html',
})
export class ProductListItemComponent {
  constructor(
    private webShop: WebshopApiClient,
  ) {}
  @Input() product!: ProductDto;
  @Input() shouldShowEditAndDeleteButton: boolean = false;

  deleteProject(): void {
    this.webShop.product_DeleteProduct(this.product.id!);
  }
}
