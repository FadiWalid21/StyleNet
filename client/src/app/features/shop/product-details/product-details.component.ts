import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatDivider } from '@angular/material/divider';
import { MatInput } from '@angular/material/input';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatDivider,
    MatLabel,
    MatFormField,
    MatInput,
    FormsModule
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent implements OnInit{
  private shopService = inject(ShopService);
  private activeRouteLink = inject(ActivatedRoute);
  private cartService = inject(CartService);
  product? :Product;

  quantityInCart = 0;
  quantity = 1;

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct(){
    const id = this.activeRouteLink.snapshot.paramMap.get('id');
    if(!id) return;

    this.shopService.getProduct(+id).subscribe({
      next : product => {
        this.product = product;
        this.updateQuantityInCart();
      },
      error : error => console.log(error)
      
    })
  }

  updateCart(){
    if(!this.product) return;

    if(this.quantity > this.quantityInCart)
    {
      const itemsToAdd = this.quantity - this.quantityInCart;
      this.quantityInCart += itemsToAdd;
      this.cartService.addItemToCart(this.product,itemsToAdd);
    }else{
      const itemsToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart -= itemsToRemove;
      this.cartService.removeItemFromCart(this.product.id,itemsToRemove);
    }
  }

  updateQuantityInCart(){
    this.quantityInCart = this.cartService.cart()?.items
    .find(i => i.productId === this.product?.id)?.quantity || 0;
    
    this.quantity = this.quantityInCart || 1;

  }

  getButtonText()
  {
    return this.quantityInCart > 0 ? 'Update cart' : 'Add to cart';
  }

}
