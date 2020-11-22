import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{
  title = 'client';
  constructor(private basketService: BasketService,
              private accountService: AccountService
    ) {}
  ngOnInit(): void {
   this.loadBasket();
   this.loadCurrentUser();
  }

  loadBasket(): void {
    const basketId = localStorage.getItem('basket_id');
    if (basketId) {
          this.basketService.getBasket(basketId).subscribe(() => {
            console.log('initialised basket');
          }, error => {
            console.log(error);
          });
    }
  }

  loadCurrentUser(): void {
   const accessToken = localStorage.getItem('token');

   this.accountService.loadCurrentUser(accessToken).subscribe(() => {
          console.log('app.components.ts', 'loaded user');
        }, error => {
          console.log(error);
        });

  }
}
