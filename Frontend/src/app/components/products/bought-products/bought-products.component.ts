import { Component, OnInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-bought-products',
  templateUrl: './bought-products.component.html',
  styleUrls: ['./bought-products.component.scss']
})
export class BoughtProductsComponent implements OnInit {
  pageIndex: number = 1;
  pageSize: number = 10;
  itemCount!: number;
  shouldShowBuyButton: boolean = false;
  listType: 'my' | 'all' | 'bought' = 'bought';

  constructor(private productsService: ProductsService) {
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService.setPageSize(this.pageSize);
  }

  ngOnInit(): void {
    this.productsService
      .fetchBoughtProducts(this.pageIndex, this.pageSize)
      .subscribe((products: any) => {
        this.itemCount = products.itemCount!;
      });
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.productsService.setPageIndex(this.pageIndex);
    this.productsService
      .fetchBoughtProducts(this.pageIndex, this.pageSize)
      .subscribe((products: any) => {
        this.itemCount = products.itemCount!;
      });
  }
}
