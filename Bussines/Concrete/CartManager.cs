using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Data.Abstract;
using Entity;

namespace Bussines.Concrete
{
    public class CartManager : ICartService
    {
        private readonly IUnitOfWork _unitofwork;
        public CartManager( IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);
            if (cart!=null)
            {
                //eklenmek istenen ürün var mı 
                var index = cart.CartItems.FindIndex(i=>i.ProductId==productId);
                if (index<0)
                {
                    cart.CartItems.Add(new CartItem(){
                        ProductId=productId,
                        Quantity=quantity,
                        CartId=cart.Id
                    });
                }else
                {
                    cart.CartItems[index].Quantity+= quantity;
                }
                _unitofwork.Carts.Update(cart);
                _unitofwork.Save();
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _unitofwork.Carts.GetByUserId(userId);
        }

        public void InitializeCart(string userId)
        {
            _unitofwork.Carts.Create(new Cart(){
                UserId = userId
            });
            _unitofwork.Save();
        }
             public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);
            if(cart!=null)
            {
                _unitofwork.Carts.DeleteFromCart(cart.Id,productId); 
                _unitofwork.Save();
            }   
        }

        public void ClearCart(int cartId)
        {
            _unitofwork.Carts.ClearCart(cartId);
            _unitofwork.Save();
        }
    }
}