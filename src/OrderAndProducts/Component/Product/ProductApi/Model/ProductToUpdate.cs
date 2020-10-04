using System.ComponentModel.DataAnnotations;

namespace ProductApi.Model
{
    public class ProductToUpdate
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(25, ErrorMessage = "Name can't be longer than 25 characters")]
        public string Name { get; set; }

        public int InStock { get; set; }

        [Required(ErrorMessage = "PictureUrl is required")]
        [StringLength(128, ErrorMessage = "Url can't be longer than 128 characters")]
        public string PictureUrl { get; set; }
    }
}
