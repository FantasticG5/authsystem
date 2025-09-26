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

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$",
        ErrorMessage = "Invalid email address")]
    // Exempel: user@example.com, test.user123@gmail.com, my-name@domain.co.uk
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(20, ErrorMessage = "Password cannot exceed 20 characters")]
    // Exempel på krav: minst 1 stor bokstav, 1 liten bokstav, 1 siffra,
    // Exempel: Test123, Hello1World, Abc123
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$",
        ErrorMessage = "Password must contain upper, and number")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmedPassword { get; set; } = null!;
}

