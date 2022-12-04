import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  IPagedListOfProductDto,
  ProductDto,
  WebshopApiClient,
} from 'src/app/generated/webshopApiClient';
import { ProductsService } from 'src/app/services/products.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss'],
})
export class ProductsListComponent implements OnInit {
  products: ProductDto[] = [];
  @Input()
  listToSubscribe: "my" | "all" | "bought" = "all";
  @Input()
  itemCount!: number;
  @Input()
  pageSize!: number;
  @Input()
  shouldShowBuyButton: boolean = true;
  @Output()
  pageIndex: EventEmitter<number> = new EventEmitter<number>();

  get hasProducts(): boolean {
    return (this.products ?? []).length > 0;
  }

  constructor(
    private productsService: ProductsService,
    private roleService: RoleService
  ) {
    this.pageIndex.arguments = 1;
  }

  ngOnInit(): void {
    switch(this.listToSubscribe){
      case "all":
        this.productsService.fetchProducts(this.pageIndex.arguments, this.pageSize).subscribe(
          (product) => { });
        this.productsService.products.subscribe(
          (products) => {
            this.products = products;
          }
        );
        break;
      case "my":
        this.productsService.fetchMyProducts(this.pageIndex.arguments, this.pageSize).subscribe(
          (product) => {});
        this.productsService.myProducts.subscribe(
          (products) => {
            this.products = products;
          }
        );
        break;
    }
  }

  get isAdmin(): boolean {
    return this.roleService.isAdmin;
  }

  handlePageEvent(pageNum: number) {
    this.pageIndex.emit(pageNum);
    switch(this.listToSubscribe){
      case "all":
        this.productsService.fetchProducts(this.pageIndex.arguments, this.pageSize).subscribe(
          (products) => {}
        );
        this.productsService.products.subscribe(
          (products) => {
            this.products = products;
          }
        );
        break;
      case "my":
        this.productsService.fetchMyProducts(this.pageIndex.arguments, this.pageSize).subscribe(
          (product) => {}
        );
        this.productsService.myProducts.subscribe(
          (products) => {
            this.products = products;
          }
        );
        break;
    }
  }
}
