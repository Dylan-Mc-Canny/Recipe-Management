using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SMS.Data.Entities;

namespace SMS.Web.Models;

public class RegisterViewModel
{     
    [Required]
    public string Name { get; set; }

    [Required] [EmailAddress]
    [Remote(action: "VerifyEmailAddress", controller: "User", ErrorMessage = "Email address is already in use. Please choose another.")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Try again !")]
    [Display(Name = "Confirm Password")]  
    public string PasswordConfirm  { get; set; }

    [Required]
    public Role Role { get; set; }
}

