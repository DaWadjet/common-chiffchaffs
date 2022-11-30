import { Injectable } from '@angular/core';
import jwt_decode from 'jwt-decode';
import AppConstants from '../common/navbar/app-constants';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  public get token() {
    return sessionStorage.getItem(AppConstants.TOKEN_KEY);
  }

  public get role() {
    if (this.token) {
      const parsedToken = jwt_decode<Token>(this.token);
      return parsedToken.role;
    }

    return null;
  }

  public get isUser() {
    return this.role === 'user';
  }

  public get isLoggedIn() {
    return !!this.token;
  }

  public get isAdmin() {
    return this.role === 'admin';
  }
}

interface Token {
  role: 'admin' | 'user';
}
