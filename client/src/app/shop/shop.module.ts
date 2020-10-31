import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopComponent } from './shop.component';
import { ProductItemComponent } from './product-item/product-item.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { ProductDetailComponent } from './product-detail/product-detail.component';



@NgModule({
  declarations: [ShopComponent, ProductItemComponent, ProductDetailComponent],
  imports: [
    CommonModule,
    SharedModule, // now we can use panination functionality
    RouterModule
  ],
  exports: [ShopComponent]
})
export class ShopModule { }
