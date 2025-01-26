import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart.service';
import { of } from 'rxjs';
import { SnakbarService } from '../services/snakbar.service';

export const emptyCartGuard: CanActivateFn = (route, state) => {

  const cartService = inject(CartService);
  const router = inject(Router);
  const snack = inject(SnakbarService);

  if(!cartService.cart() || cartService.cart()?.items.length === 0)
  {
    snack.error('Your cart is empty');
    router.navigateByUrl('/cart');
    return false;
  }
  return true;
};
