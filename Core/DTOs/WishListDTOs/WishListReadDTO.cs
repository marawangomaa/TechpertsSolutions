using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechpertsSolutions.Core.Entities;

namespace Core.DTOs.WishListDTOs
{
    public class WishListReadDTO
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WishListItemReadDTO> Items { get; set; } = new();
        //public WishListReadDTO(WishList wishList) 
        //{
        //    Id = wishList.Id;
        //    CustomerId = wishList.CustomerId;
        //    CreatedAt = wishList.CreatedAt;
        //    Items = wishList.WishListItems.Select(item => new WishListItemReadDTO(item)).ToList();
        //}
    }
}
