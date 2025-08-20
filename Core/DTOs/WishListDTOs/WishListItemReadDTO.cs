using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.WishListDTOs
{
    public class WishListItemReadDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string WishListId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string? ProductImageUrl { get; set; }
        //public WishListItemReadDTO(WishListItem wishListItem)  
        //{
        //    Id = wishListItem.Id;
        //    ProductId = wishListItem.ProductId;
        //    WishListId = wishListItem.WishListId;
        //    //ProductName = wishListItem.ProductName;
        //    //ProductPrice = wishListItem.ProductPrice;
        //    //ProductImageUrl = wishListItem.ProductImageUrl;
        //}
    }
}
