import { Component, OnInit } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { IPagedListOfProductDto, WebshopApiClient } from 'src/app/generated/webshopApiClient';
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
    private roleService: RoleService) {
      console.log("aaa");
      this.webShop.product_ListProducts(this.pageIndex,this.pageSize).pipe(
        tap((products) => {
          this.products = products;
          console.log("aaaa");
          console.log(products);
          this.itemCount = products.itemCount!;
        })
      );
    }

  ngOnInit(): void {
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    this.webShop.product_ListProducts(this.pageIndex,this.pageSize).pipe(
      tap((products) => {
        this.products = products;
      })
    );
    // we emit Index and size together so we need the query in one function
  }

  // onPageSizeChange(pageSize: number) {
  //   this.pageSize = pageSize;
  //   //this.auctionService.getOwnAuctionCount(this.userId, res => this.ownItemCount = res)
  //   //this.auctionService.getOwnAuctions(this.userId, this.ownPageSize, this.ownPageIndex, res2 => this.ownedAuctions = res2);
  // }
}
