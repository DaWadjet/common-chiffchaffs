import { Component, OnInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-products-home',
  templateUrl: './products-home.component.html',
  styleUrls: ['./products-home.component.scss'],
})
export class ProductsHomeComponent implements OnInit {
  pageIndex: number = 1;
  pageSize: number = 10;
  itemCount!: number;
  shouldShowBuyButton: boolean = true;
  listType: 'my' | 'all' | 'bought' = 'all';

  constructor(private productsService: ProductsService) {
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService.setPageSize(this.pageSize);
  }

  ngOnInit(): void {
    this.productsService
      .fetchProducts(this.pageIndex, this.pageSize)
      .subscribe((products) => {
        this.itemCount = products.itemCount!;
      });
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService
      .fetchProducts(this.pageIndex, this.pageSize)
      .subscribe((products) => {
        this.itemCount = products.itemCount!;
      });
  }
}
