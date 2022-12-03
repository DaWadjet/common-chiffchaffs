import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductsListComponent } from './components/products-list/products-list.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { AuthGuard } from './guard/auth.guard';
import { SingleProductService } from './resolvers/single-product.service';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'products/:productId',
        resolve: [SingleProductService],
        component: ProductDetailsComponent,
      },
      {
        path: '',
        component: ProductsListComponent,
      },
      { path: '**', redirectTo: '/' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
