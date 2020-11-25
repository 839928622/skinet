import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderComponent } from './order.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { OrderRoutingModule } from './order-routing.module';




@NgModule({
  declarations: [OrderComponent, OrderDetailComponent, ],
  imports: [
    CommonModule,
    OrderRoutingModule,
  ]
})
export class OrderModule { }
