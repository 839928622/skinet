import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrderService } from '../order.service';

@Component({
  selector: 'app-order-detail',
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.scss']
})
export class OrderDetailComponent implements OnInit {
  order: IOrder;
  constructor(private activateRoute: ActivatedRoute, private breadCrumbService: BreadcrumbService,
              private orderService: OrderService) {
                this.breadCrumbService.set('@OrderDetail', ' ');
               }

  ngOnInit(): void {
    this.orderService.getOrderDetail(+this.activateRoute.snapshot.paramMap.get('orderId')).
    subscribe((order: IOrder) => {
      this.order = order;
      this.breadCrumbService.set('@OrderDetail', `Order# ${order.id} - ${order.status}`);
    }, error => {
      console.log(error);
    });
  }

}
