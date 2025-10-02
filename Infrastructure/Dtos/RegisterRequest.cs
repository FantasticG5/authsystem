using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure.Dtos;

public class RegisterRequest
{
    [Required(ErrorMessage = "Firstname is required")]
    [MinLength(2, ErrorMessage = "Firstname must be at least 2 characters")]
    [MaxLength(50, ErrorMessage = "Firstname cannot exceed 50 characters")]
    public string Firstname { get; set; } = null!;

    [Required(ErrorMessage = "Lastname is required")]
    [MinLength(2, ErrorMessage = "Lastname must be at least 2 characters")]
    [MaxLength(50, ErrorMessage = "Lastname cannot exceed 50 characters")]
    public string Lastname { get; set; } = null!;

    [Required(ErrorMessage = "You must enter your email address.")]
    [RegularExpression(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$", ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(20, ErrorMessage = "Password cannot exceed 20 characters")]
    // Tillåt valfria tecken, men kräv minst 1 bokstav och 1 siffra (6–20 tecken)
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,20}$",
        ErrorMessage = "Password must be 6–20 characters and include at least one letter and one digit")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmedPassword { get; set; } = null!;
}

