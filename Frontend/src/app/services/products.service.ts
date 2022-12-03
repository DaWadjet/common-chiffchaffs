import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { ProductDto, WebshopApiClient } from '../generated/webshopApiClient';

@Injectable({
  providedIn: 'root',
})
export class ProductsService {
  constructor(private api: WebshopApiClient) {}

  myProducts = new BehaviorSubject<ProductDto[]>([]);
  products = new BehaviorSubject<ProductDto[]>([]);
  selectedProduct = new BehaviorSubject<ProductDto | undefined>(undefined);

  clearProducts() {
    this.selectedProduct.next(undefined);
    this.myProducts.next([]);
    this.products.next([]);
  }

  fetchProductById(productId: string) {
    return this.api.product_GetProductDetails(productId).pipe(
      tap((product) => {
        this.selectedProduct.next(product);
      })
    );
  }
}
