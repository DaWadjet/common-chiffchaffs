import { Component, OnInit } from '@angular/core';
import { WebshopApiClient } from 'src/app/generated/webshopApiClient';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-products-home',
  templateUrl: './products-home.component.html',
  styleUrls: ['./products-home.component.scss']
})
export class ProductsHomeComponent implements OnInit {
  pageIndex: number = 0;
  pageSize: number = 10;

  constructor(
    private webShop: WebshopApiClient,
    private roleService: RoleService) { }

  ngOnInit(): void {
  }

  onIndexChange(index: number) {
    this.pageIndex = index;
    // we emit Index and size together so we need the query in one function
  }

  onPageSizeChange(pageSize: number) {
    this.pageSize = pageSize;
    //this.webShop.
    //this.auctionService.getOwnAuctionCount(this.userId, res => this.ownItemCount = res)
    //this.auctionService.getOwnAuctions(this.userId, this.ownPageSize, this.ownPageIndex, res2 => this.ownedAuctions = res2);
  }
}
