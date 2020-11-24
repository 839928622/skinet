import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CheckoutComponent } from './checkout.component';
import { CheckoutRoutingModule } from './checkout-routing.module';
import { SharedModule } from '../shared/shared.module';
import { CheckoutDeliveryComponent } from './checkout-delivery/checkout-delivery.component';
import { CheckoutReviewComponent } from './checkout-review/checkout-review.component';
import { CheckoutPaymentComponent } from './checkout-payment/checkout-payment.component';
import { CheckoutSucessComponent } from './checkout-sucess/checkout-sucess.component';
import { CheckoutAddressComponent } from './checkout-address/checkout-address.component';



@NgModule({
  declarations: [CheckoutComponent, CheckoutDeliveryComponent,
                 CheckoutReviewComponent, CheckoutPaymentComponent,
                 CheckoutSucessComponent, CheckoutAddressComponent],
  imports: [
    CommonModule,
    CheckoutRoutingModule,
    SharedModule,
  ]
})
export class CheckoutModule { }
