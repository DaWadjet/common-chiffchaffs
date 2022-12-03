import { Injectable } from '@angular/core';
import { BehaviorSubject, flatMap, tap } from 'rxjs';
import {
  FileParameter,
  ProductDto,
  WebshopApiClient,
} from '../generated/webshopApiClient';

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

  //todo write this in a correctly paged way
  fetchProducts() {
    return this.api.product_ListProducts(1, 10).pipe(
      tap((products) => {
        this.products.next(products.items ?? []);
      })
    );
  }

  createProduct(
    name: string,
    description: string,
    price: number,
    caffFile: FileParameter
  ) {
    return this.api
      .product_SaveProduct(name, description, price, caffFile)
      .pipe(flatMap(() => this.fetchProducts()));
  }

  updateProduct(name: string, description: string, price: number, id: string) {
    return this.api
      .product_UpdateProduct({ name, description, price, id })
      .pipe(flatMap(() => this.fetchProducts()));
  }
}
