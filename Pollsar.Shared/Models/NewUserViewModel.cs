using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pollsar.Shared.Models
{
    public class NewUserViewModel : BaseViewModel
    {
        private string names;

        [Display(Name = "Names")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} is requried")]
        public string Names
        {
            get => names;
            set
            {
                var temp = names;
                names = value;
                if (temp == value) return;
                OnPropertyChanged();
            }
        }

        private string email;

        [EmailAddress(ErrorMessage = "{0} is invalid")]
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Email
        {
            get => email;
            set
            {
                var temp = email;
                email = value;
                if (temp == value) return;
                OnPropertyChanged();
            }
        }

        private string password;

        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "{0} can only range from {2} to {1} characters in length")]
        public string Password
        {
            get => password;
            set
            {
                var temp = password;
                password = value;
                if (temp == value) return;
                OnPropertyChanged();
            }
        }

        private string passwordConfirm;

        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        [Display(Name = "Confirm Password")]
        [StringLength(100, ErrorMessage = "{0} can only range from {2} to {1} characters in length")]
        [Compare("Password", ErrorMessage = "Passwords mismatch")]
        public string PasswordConfirm
        {
            get => passwordConfirm;
            set
            {
                var temp = passwordConfirm;
                passwordConfirm = value;
                if (temp == value) return;
                OnPropertyChanged();
            }
        }
    }
}
