import { Component, OnInit } from '@angular/core';
import { IPagedListOfProductDto, WebshopApiClient } from 'src/app/generated/webshopApiClient';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrls: ['./my-products.component.scss']
})
export class MyProductsComponent implements OnInit {
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
    this.productsService.fetchMyProducts(this.pageIndex, this.pageSize).subscribe(
      (product) => {
        this.products = product!;
        this.itemCount = this.products.itemCount!;
      }
    );
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.productsService.fetchMyProducts(this.pageIndex, this.pageSize).subscribe(
      (product) => {
        this.products = product!;
        this.itemCount = this.products.itemCount!;
      }
    );
      this.productsService.setPageIndex(this.pageIndex);

  }

}
