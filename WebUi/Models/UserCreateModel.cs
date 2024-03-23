using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace WebUi.Models
{
      public class UserCreateModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        public bool IsStripeCustomer {get;set;}
        public List<string> SelectedRoles { get; set; } 
        public List<string> AllRoles { get; set; }
        [Required(ErrorMessage = "Please select a profile picture.")]
        [Display(Name = "Profile Picture")]
        public string SelectedProfilePicture { get; set; }

        public List<string> AvailableProfilePictures { get; } = new List<string>
        {
            "avatar(w).jpg",
            "avatar(m).jpg"
        };
    }
}