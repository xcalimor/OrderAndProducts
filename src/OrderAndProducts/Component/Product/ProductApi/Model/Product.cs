using System;

namespace ProductApi.Model
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int InStock { get; set; }
        public string PictureUrl { get; set; }
    }
}
