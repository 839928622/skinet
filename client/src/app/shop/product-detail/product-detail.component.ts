import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BasketService } from 'src/app/basket/basket.service';
import { IProduct } from 'src/app/shared/models/product';
import { BreadcrumbService } from 'xng-breadcrumb';
import { ShopService } from '../shop.service';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit {
product: IProduct;
quantity = 1;
  constructor(private shopServive: ShopService,
              private activaRoute: ActivatedRoute,
              private breadCrumbService: BreadcrumbService,
              private basketService: BasketService,
              private toastr: ToastrService,
              ) {
                this.breadCrumbService.set('@prodectDetails', ' '); // must set to length >= 1 of string,if u set it to '',it won't work,
                                                                    // at least at current version,it won't
               }

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    this.shopServive.getProduct(+this.activaRoute.snapshot.paramMap.get('id')).subscribe(response => {
      this.product = response;
      this.breadCrumbService.set('@prodectDetails', this.product.name);
    }, error => {
      console.log(error);
    });
  }


  incrementItemQuantity(): void{
   this.quantity++;
  }


  decrementItemQuantity(): void{
    if (this.quantity > 1)
    {
      this.quantity--;
    }else {
      this.quantity = 1;
    }
  }

  addItemToBasket(): void {
    this.basketService.addItemToBasket(this.product, this.quantity);
    this.toastr.success(this.product.name + (this.quantity > 1 ? ' have' : ' has') + ' been successfully added to your shopping cart');
  }

}
