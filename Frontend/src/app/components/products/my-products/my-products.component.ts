import { Component, OnInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-my-products',
  templateUrl: './my-products.component.html',
  styleUrls: ['./my-products.component.scss'],
})
export class MyProductsComponent implements OnInit {
  pageIndex: number = 1;
  pageSize: number = 10;
  itemCount!: number;
  shouldShowBuyButton: boolean = false;
  listType: 'my' | 'all' | 'bought' = 'my';

  constructor(private productsService: ProductsService) {
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService.setPageSize(this.pageSize);
  }

  ngOnInit(): void {
    this.productsService
      .fetchMyProducts(this.pageIndex, this.pageSize)
      .subscribe((products: any) => {
        this.itemCount = products.itemCount!;
      });
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService
      .fetchMyProducts(this.pageIndex, this.pageSize)
      .subscribe((products: any) => {
        this.itemCount = products.itemCount!;
      });
  }
}
