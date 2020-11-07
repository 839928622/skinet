import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Basket, IBasket, IBasketItem, IBasketTotal } from '../shared/models/basket';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
baseUrl: string = environment.apiUrl;

private basketSource = new BehaviorSubject<IBasket>(null);
basket$ = this.basketSource.asObservable();

private basketTotalSource = new BehaviorSubject<IBasketTotal>(null);
basketTotal$ = this.basketTotalSource.asObservable();

  constructor(private http: HttpClient) { }

  getBasket(id: string) {
    return this.http.get<IBasket>(this.baseUrl + 'basket?id=' + id).pipe(
      map((basket: IBasket) => {
         this.basketSource.next(basket);
         this.caculateTotals();
      })
    );
  }

  // push basket to api and store it with redis
  // at the same time, cast a message to whom  interested in
  setBasket(basket: IBasket) {
    return this.http.post(this.baseUrl + 'basket', basket).subscribe(
      (response: IBasket) => {
        this.basketSource.next(response);
        this.caculateTotals();
      }, error => {
        console.log(error);
      }
    );
  }

  getCurrentBasketValue(): IBasket {
    return this.basketSource.value;
  }

  addItemToBasket(item: IProduct, quantity: number = 1) {
  const itemToAdd: IBasketItem = this.mapProductItemToBasketItem(item, quantity);
  const basket = this.getCurrentBasketValue() ?? this.createBasket();
  basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
  this.setBasket(basket);
  }

  //
  private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {

    const index = items.findIndex( x => x.id === itemToAdd.id);
    if (index === -1) { // not found that specific item
       itemToAdd.quantity = quantity;
       items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }
    return items;
  }
  private createBasket(): IBasket {
    const basket = new Basket();
    localStorage.setItem('basket_id', basket.id);
    return basket;
  }
  private mapProductItemToBasketItem(item: IProduct, quantity: number): IBasketItem {
    return {
       id: item.id,
       productName: item.name,
       price: item.price,
       pictureUrl: item.pictureUrl,
       quantity, // it means quantity = quantity
       brand: item.productBrand,
       type: item.productType

    };
  }

  // caculate product totals like amount
  private caculateTotals() {
    const currentBasket = this.getCurrentBasketValue();
    const shipping = 0;
    const subTotal = currentBasket.items.reduce((a, b) => (b.price * b.quantity) + a, 0);
    const total = subTotal + shipping;
    this.basketTotalSource.next({
      shipping, total, subTotal});
  }

  incrementItemQuantity(item: IBasketItem ){
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    basket.items[foundItemIndex].quantity ++;
    this.setBasket(basket);
  }


  decrementItemQuantity(item: IBasketItem ){
    const basket = this.getCurrentBasketValue();
    const foundItemIndex = basket.items.findIndex(x => x.id === item.id);
    if ( basket.items[foundItemIndex].quantity > 1) {
      basket.items[foundItemIndex].quantity --;
      this.setBasket(basket);
    } else {
      this.removeItemFromBasket(item);
    }
  }

   removeItemFromBasket(item: IBasketItem) {
    const basket = this.getCurrentBasketValue();
    if (basket.items.some(x => x.id === item.id)) {
      basket.items = basket.items.filter(x => x.id !== item.id);
      if (basket.items.length > 0) {
        this.setBasket(basket);
      }else {
        this.deleteBasket(basket);
      }
    }
  }

  deleteBasket(basket: IBasket) {
    return this.http.delete(this.baseUrl + 'basket?basketId=' + basket.id).subscribe( () => {
      this.basketSource.next(null);
      this.basketTotalSource.next(null);
      localStorage.removeItem('basket_id');
    }, error => {
      console.log(error);
    });
  }

}
