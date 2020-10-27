import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
baseUrl: string = environment.apiUrl;
  constructor(private httpClient: HttpClient,
              ) { }

  getProducts(): Observable<IPagination<IProduct>> {
    return this.httpClient.get<IPagination<IProduct>>(this.baseUrl + 'products/products');
  }
}
