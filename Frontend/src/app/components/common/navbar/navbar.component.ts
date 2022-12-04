import { Component, OnInit } from '@angular/core';
import { LogoutService } from 'src/app/services/logout.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent implements OnInit {
  constructor(private logoutService: LogoutService) {}

  ngOnInit(): void {}

  onLogout() {
    this.logoutService.logout();
  }
}
