import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IBrand } from '../shared/models/brand';
import { IProduct } from '../shared/models/product';
import { IProductType } from '../shared/models/productType';
import { ShopParams } from '../shared/models/shopParams';
import { ShopService } from './shop.service';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search', {static: true}) searchTerm: ElementRef ;
products: IProduct[];
brands: IBrand[];
productTyps: IProductType[];
shopParams = new ShopParams();
totalCount: number;
sortOptions = [{name: 'Aplhabetical', value: 'name'}, {name: 'Price: Low to High', value: 'priceAsc'}, {name: 'Price: High to Low', value: 'priceDesc'}];
  constructor(private shopService: ShopService) { }

  ngOnInit(): void {
   this.getProducts();
   this.getBrands();
   this.getProductTypes();
  }

  getProducts(): void {
    this.shopService.getProducts(this.shopParams).subscribe(response => {
      this.products = response.data;
      this.totalCount = response.count;
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
    this.shopParams.brandId = brandId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    this.shopParams.typeId = typeId;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onSortChange(sort: string) {
    console.log('eventTarget.value:', sort);
    this.shopParams.sort = sort;
    this.getProducts();
  }

  onPageChanged(event: any){
    console.log('event', event);
    this.shopParams.pageNumber = event.page;
    this.getProducts();
  }

  onSearch() {
    this.shopParams.search = this.searchTerm.nativeElement.value;
    this.shopParams.pageNumber = 1;
    this.getProducts();
  }

  onReset() {
    this.searchTerm.nativeElement.value = '';
    this.shopParams = new ShopParams();
    this.getProducts();
  }

}
