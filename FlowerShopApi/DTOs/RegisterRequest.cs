using System.ComponentModel.DataAnnotations;

namespace FlowerShopApi.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; }

        [Phone]
        [Required(ErrorMessage = "Phone is required")]
        public string Phone {  get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
