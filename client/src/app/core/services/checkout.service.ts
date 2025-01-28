import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  delivryMethods : DeliveryMethod[] = [];

  getDeliveryMethods(){
    if(this.delivryMethods.length > 0) return of(this.delivryMethods);

    return this.http.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-methods').pipe(
      map(methods=> {
        this.delivryMethods = methods.sort((a,b) => b.price - a.price);
        return methods;
      })
    );
  }
}
