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
  pageIndex: number = 1;
  pageSize: number = 10;

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

  fetchProducts(pageIndex: number, pageSize: number) {
    return this.api.product_ListProducts(pageIndex, pageSize).pipe(
      tap((products) => {
        this.products.next(products.items ?? []);
      })
    );
  }

  fetchMyProducts(pageIndex: number, pageSize: number) {
    return this.api.product_ListProducts2(pageIndex, pageSize).pipe(
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
      .pipe(flatMap(() => this.fetchProducts(this.pageIndex, this.pageSize)));
  }

  updateProduct(name: string, description: string, price: number, id: string) {
    return this.api
      .product_UpdateProduct({ name, description, price, id })
      .pipe(flatMap(() => this.fetchProducts(this.pageIndex, this.pageSize)));
  }

  deleteProduct(productId: string) {
    return this.api.product_DeleteProduct(productId).pipe(
      tap(() => {
        this.products.next(
          this.products.value.filter((p) => p.id !== productId)
        );
        this.myProducts.next(
          this.myProducts.value.filter((p) => p.id !== productId)
        );
      })
    );
  }

  buyProduct(productId: string) {
    return this.api.product_BuyProduct(productId).pipe
    (tap(() => {
      this.products.next(
        this.products.value.filter((p) => p.id !== productId)
      );
    }));
  }

  setPageIndex(pageIndex: number) {
    this.pageIndex = pageIndex;
  }
  setPageSize(pageSize: number) {
    this.pageSize = pageSize;
  }
}