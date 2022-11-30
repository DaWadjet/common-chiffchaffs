import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { RoleService } from '../services/role.service';

@Injectable({
  providedIn: 'root',
})
export class AdminOnlyGuard implements CanActivate {
  constructor(private roleService: RoleService) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    return this.roleService.isAdmin;
  }
}
