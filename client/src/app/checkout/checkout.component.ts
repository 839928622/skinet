import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup;
  constructor(private fromBuilder: FormBuilder, private accountService: AccountService) { }

  ngOnInit(): void {
    this.createCheckoutForm();
    this.getAddressFormValues();
  }


  createCheckoutForm() {
    this.checkoutForm = this.fromBuilder.group({
      addressForm: this.fromBuilder.group({
        firstName: [null, Validators.required],
        lastName:  [null, Validators.required],
        street:    [null, Validators.required],
        city:      [null, Validators.required],
        state:     [null, Validators.required],
        zipcode:   [null, Validators.required],
        }),
      deliveryForm: this.fromBuilder.group({
        deliveryMethod: [null, Validators.required]
      }),
      paymentForm: this.fromBuilder.group({
        nameOnCard: [null, Validators.required]
      }),
    });
  }

  getAddressFormValues() {
    this.accountService.getUserAddress().subscribe(address => {
      if (address) {
        this.checkoutForm.get('addressForm').patchValue(address);
      }
    }, error => {
      console.log(error);
    });
  }
}
