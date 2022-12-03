import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable } from 'rxjs';
import { ProductDto } from '../generated/webshopApiClient';
import { ProductsService } from './../services/products.service';

@Injectable({
  providedIn: 'root',
})
export class SingleProductService implements Resolve<ProductDto> {
  constructor(private productsService: ProductsService) {}
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): ProductDto | Observable<ProductDto> | Promise<ProductDto> {
    const currentlyLoadedProduct = this.productsService.selectedProduct.value;
    const productId = route.params['productId'];
    if (currentlyLoadedProduct?.id === productId) {
      return currentlyLoadedProduct!;
    } else {
      return this.productsService.fetchProductById(productId);
    }
  }
}
