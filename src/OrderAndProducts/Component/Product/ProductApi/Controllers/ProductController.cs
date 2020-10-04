using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Model;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet("{id}")]
        public IEnumerable<Product> Get(string id)
        {
            var listOfProduct = new List<Product>();

            listOfProduct.Add(new Product
            {
                Id = Guid.Parse("0993D8F7-9FFC-4C6D-8249-349E1A640F54"),
                InStock = 10,
                Name = "Playstation 5",
                PictureUrl = "2314"
            });

            return listOfProduct;
        }

        [HttpGet()]
        //[Authorize(Roles = "Admin,User")]
        public IEnumerable<Product> Get()
        {
            var listOfProduct = new List<Product>();
            listOfProduct.Add(new Product
            {
                Id = Guid.Parse("87816CA2-A45B-436B-97B7-2625B4111958"),
                InStock = 10,
                Name = "Xbox Series X",
                PictureUrl = "2314"
            });
            listOfProduct.Add(new Product
            {
                Id = Guid.Parse("1945295F-4AFC-4B86-B99A-209D99352B32"),
                InStock = 10,
                Name = "Xbox series S",
                PictureUrl = "2314"
            });
            listOfProduct.Add(new Product
            {
                Id = Guid.Parse("0993D8F7-9FFC-4C6D-8249-349E1A640F54"),
                InStock = 10,
                Name = "Playstation 5",
                PictureUrl = "2314"
            });
            listOfProduct.Add(new Product
            {
                Id = Guid.Parse("1CA1531F-91DF-45C5-8E08-D4920C78AC95"),
                InStock = 10,
                Name = "Amico",
                PictureUrl = "2314"
            });
            listOfProduct.Add(new Product
            {
                Id = Guid.Parse("8DA2875B-5ACA-4EC0-B884-6C4AAC63527F"),
                InStock = 10,
                Name = "Nintendo Switch",
                PictureUrl = "2314"
            });
            
            return listOfProduct;
        }
    }
}
