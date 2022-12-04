import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
  IPagedListOfProductDto,
  WebshopApiClient,
} from 'src/app/generated/webshopApiClient';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss'],
})
export class ProductsListComponent implements OnInit {
  @Input()
  products!: IPagedListOfProductDto;
  @Input()
  itemCount!: number;
  @Input()
  pageSize!: number;
  @Output()
  pageIndex: EventEmitter<number> = new EventEmitter<number>();

  constructor(
    private webShop: WebshopApiClient,
    private roleService: RoleService
  ) {
    this.pageIndex.arguments = 1;
  }

  ngOnInit(): void {}

  get isAdmin(): boolean {
    return this.roleService.isAdmin;
  }

  handlePageEvent(pageNum: number) {
    this.pageIndex.emit(pageNum);
  }
}
