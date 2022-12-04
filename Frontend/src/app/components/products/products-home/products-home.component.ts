import { Component, OnInit } from '@angular/core';
import { IPagedListOfProductDto } from 'src/app/generated/webshopApiClient';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-products-home',
  templateUrl: './products-home.component.html',
  styleUrls: ['./products-home.component.scss'],
})
export class ProductsHomeComponent implements OnInit {
  pageIndex: number = 1;
  pageSize: number = 10;
  products!: IPagedListOfProductDto;
  itemCount!: number;
  shouldShowBuyButton: boolean = true;

  constructor(private productsService: ProductsService) {
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService.setPageSize(this.pageSize);
  }

  ngOnInit(): void {
    this.productsService
      .fetchProducts(this.pageIndex, this.pageSize)
      .subscribe((product) => {
        this.products = product!;
        this.itemCount = this.products.itemCount!;
      });
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.productsService
      .fetchProducts(this.pageIndex, this.pageSize)
      .subscribe((product) => {
        this.products = product!;
        this.itemCount = this.products.itemCount!;
      });
    this.productsService.setPageIndex(this.pageIndex);
  }
}
