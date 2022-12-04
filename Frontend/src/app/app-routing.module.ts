import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MyProductsComponent } from './components/products/my-products/my-products.component';
import { ProductDetailsComponent } from './components/products/product-details/product-details.component';
import { ProductFormComponent } from './components/products/product-form/product-form.component';
import { ProductsHomeComponent } from './components/products/products-home/products-home.component';
import { AuthGuard } from './guard/auth.guard';
import { SingleProductService } from './resolvers/single-product.service';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'products',
        component: ProductsHomeComponent,
      },
      {
        path: 'products/new',
        component: ProductFormComponent,
      },
      {
        path: 'products/:productId',
        resolve: [SingleProductService],
        component: ProductDetailsComponent,
      },
      {
        path: 'products/:productId/edit',
        resolve: [SingleProductService],
        component: ProductFormComponent,
      },
      {
        path: 'my-products',
        component: MyProductsComponent,
      },
      {
        path: '',
        redirectTo: 'products',
        pathMatch: 'full',
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
