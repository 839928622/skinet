import { IDeliveryMethod } from '../shared/models/deliveryMethod';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IOrderToCreate } from '../shared/models/order';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getDeliveryMethods() {
    return this.http.get(this.baseUrl + 'orders/deliveryMethods').pipe(
      map( (deliveryMethod: IDeliveryMethod[]) => {
        return deliveryMethod.sort((a, b) => b.price - a.price);
      })
    );
  }
  // API will returen a created order but we dont really need to handle that
  createOrder(order: IOrderToCreate): Observable<object> {
    return this.http.post(this.baseUrl + 'orders', order);
  }
}
