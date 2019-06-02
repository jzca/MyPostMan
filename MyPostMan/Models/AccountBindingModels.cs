using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MyPostMan.Models
{
    // Models used as parameters to AccountController actions.

    public class LoginBindingModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {

        [Display(Name = "Email")]
        public string Email { get; set; }
     
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordBindingModel
    {

        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ResetPasswordBindingModel
    {

        [Display(Name = "Email")]
        public string Email { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class RegisterExternalBindingModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}
