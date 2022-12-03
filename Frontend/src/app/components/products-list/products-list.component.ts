import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IPagedListOfProductDto, WebshopApiClient } from 'src/app/generated/webshopApiClient';
import { RoleService } from 'src/app/services/role.service';
import { ProductListItemComponent } from './product-list-item/product-list-item.component';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss'],
})
export class ProductsListComponent implements OnInit {
  @Input()
  products: IPagedListOfProductDto[] = [];
  @Input()
  itemCount!: number;
  @Output()
  pageIndex: EventEmitter<number> = new EventEmitter<number>();
  pageSize: EventEmitter<number> = new EventEmitter<number>();

  constructor(
    private webShop: WebshopApiClient,
    private roleService: RoleService
  ) {
  }

  ngOnInit(): void {}

  get isAdmin(): boolean {
    return this.roleService.isAdmin;
  }

  handlePageEvent(e: onPageChange) {
    this.pageIndex.emit(e.number);
    this.pageSize.emit(e.size);
}
}
