import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';

@Component({
  selector: 'app-checkout-sucess',
  templateUrl: './checkout-sucess.component.html',
  styleUrls: ['./checkout-sucess.component.scss']
})
export class CheckoutSucessComponent implements OnInit {
order: IOrder;
  constructor(private router: Router) { const navigation = this.router.getCurrentNavigation();
                                        const state = navigation && navigation.extras && navigation.extras.state;
                                        if (state) {
                                          this.order = state as IOrder;
                                        }}

  ngOnInit(): void {
  }

}
