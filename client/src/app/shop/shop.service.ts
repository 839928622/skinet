import { IProductType } from '../shared/models/productType';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IBrand } from '../shared/models/brand';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { delay, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
baseUrl: string = environment.apiUrl;
  constructor(private httpClient: HttpClient,
              ) { }

  getProducts(brandId?: number, typeId?: number): Observable<IPagination<IProduct>> {
    let params = new HttpParams();
    if ( brandId) {
    // if branchId === 0 ,it return false
      params = params.append('brandId', brandId.toString());
    }
    if ( typeId) {
      params = params.append('typeId', typeId.toString());
    }
    return this.httpClient.get<IPagination<IProduct>>(this.baseUrl + 'products/products', {observe: 'response', params})
    .pipe(delay(500), map(response => {
      return response.body;
    }));
  }

  getBrands(): Observable<IBrand[]> {
    return this.httpClient.get<IBrand[]>(this.baseUrl + 'products/brands');
  }

  getPruductTypes(): Observable<IProductType[]> {
    return this.httpClient.get<IProductType[]>(this.baseUrl + 'products/types');
  }
}
