import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IOrder } from '../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  constructor(private httpClient: HttpClient) { }

  getOrdersForUser(): Observable<IOrder[]> {
    return this.httpClient.get<IOrder[]>(this.baseUrl + 'orders');
  }

  getOrderDetail(orderId: number): Observable<IOrder> {
    return this.httpClient.get<IOrder>( this.baseUrl + 'orders/' + orderId );
  }
}
