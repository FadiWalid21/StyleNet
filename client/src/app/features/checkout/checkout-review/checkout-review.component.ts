import { CurrencyPipe } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { CartService } from '../../../core/services/cart.service';
import { ConfirmationToken } from '@stripe/stripe-js';
import { AddressPipe } from "../../../shared/pips/address.pipe";
import { PaymentCardPipe } from "../../../shared/pips/payment-card.pipe";

@Component({
  selector: 'app-checkout-review',
  imports: [
    CurrencyPipe,
    AddressPipe,
    PaymentCardPipe
],
  templateUrl: './checkout-review.component.html',
  styleUrl: './checkout-review.component.scss'
})
export class CheckoutReviewComponent {
  cartService = inject(CartService);
  @Input() confirmationToken? : ConfirmationToken
}
