import { v4 as uuidv4 } from 'uuid';
export interface IBasket {
    id: string;
    items: IBasketItem[];
  }

export interface IBasketItem {
    id: number;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
    brand: string;
    type: string;
  }

export class Basket implements IBasket {
    id = uuidv4();
    items: IBasketItem[] = []; // initial value like List<T> TList = New List<T>() or it will throw undefined

  }

export interface IBasketTotal {
    shipping: number;
    subTotal: number;
    total: number;
  }
