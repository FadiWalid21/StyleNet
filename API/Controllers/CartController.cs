using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController(ICartRepository repo) : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
        {
            var cart = await repo.GetCartAsync(id);

            return Ok(cart ?? new ShoppingCart{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart){
            var updatedCart = await repo.SetCartAsync(cart);
            if(updatedCart is null) return BadRequest("Problem with cart");

            return Ok(updatedCart);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCart(string id)
        {
            var result = await repo.DeleteCartAsync(id);
            if(!result) return BadRequest("Problem deleting cart");

            return Ok();
        }
    }
}