import { Component, OnInit } from '@angular/core';
import { map, Observable, tap } from 'rxjs';
import { IPagedListOfProductDto, WebshopApiClient } from 'src/app/generated/webshopApiClient';
import { ProductsService } from 'src/app/services/products.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-products-home',
  templateUrl: './products-home.component.html',
  styleUrls: ['./products-home.component.scss']
})
export class ProductsHomeComponent implements OnInit {
  pageIndex: number = 1;
  pageSize: number = 10;
  products!: IPagedListOfProductDto;
  itemCount!: number;

  constructor(
    private webShop: WebshopApiClient,
    private productsService: ProductsService) {
      this.productsService.setPageIndex(this.pageIndex);
      this.productsService.setPageSize(this.pageSize);
    }

  ngOnInit(): void {
    this.productsService.fetchProducts(this.pageIndex, this.pageSize).subscribe(
      (product) => {
        this.products = product!;
        this.itemCount = this.products.itemCount!;
      }
    );
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.productsService.fetchProducts(this.pageIndex, this.pageSize).subscribe(
      (product) => {
        this.products = product!;
        this.itemCount = this.products.itemCount!;
      }
    );
      this.productsService.setPageIndex(this.pageIndex);

  }

}
