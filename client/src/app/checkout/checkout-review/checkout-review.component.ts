import { CdkStepper } from '@angular/cdk/stepper';
import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subscription } from 'rxjs';
import { BasketService } from 'src/app/basket/basket.service';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent implements OnInit {
@Input() appStepper: CdkStepper;
  constructor(private basketService: BasketService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

   createPaymentIntent(): Subscription {
     return this.basketService.createPaymentIntent().subscribe((response) => {
        // this.toastr.success('Payment intent created');
        this.appStepper.next();
     }, error => {
       this.toastr.error(error.message);
     });
   }
}
