import { Component, OnInit } from '@angular/core';
import { error } from 'protractor';
import { IBrand } from '../shared/models/brand';
import { IProduct } from '../shared/models/product';
import { IProductType } from '../shared/models/productType';
import { ShopService } from './shop.service';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
products: IProduct[];
brands: IBrand[];
productTyps: IProductType[];
brandIdSelected: number;
typeIdSelected: number;
  constructor(private shopService: ShopService) { }

  ngOnInit(): void {
   this.getProducts();
   this.getBrands();
   this.getProductTypes();
  }

  getProducts(): void {
    this.shopService.getProducts(this.brandIdSelected, this.typeIdSelected).subscribe(response => {
      this.products = response.data;
    }, errors => {
      console.log(errors);
    });
  }

  getBrands(): void {
    this.shopService.getBrands().subscribe(response => {
      this.brands = [{id: 0, name: 'all'}, ...response];
      console.log(response);
    }, errors => {
      console.log(errors);
    });
  }

  getProductTypes(): void {
    this.shopService.getPruductTypes().subscribe(response => {
      this.productTyps = [{id: 0, name: 'all'}, ...response];
    }, errors => {
      console.log(errors);
    });
  }

  onBrandSelected(brandId: number) {
    this.brandIdSelected = brandId;
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    this.typeIdSelected = typeId;
    this.getProducts();
  }



}
