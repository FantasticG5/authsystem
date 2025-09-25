using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dtos;

public class RegisterRequest
{

    [Required]

    public string Firstname { get; set; } = null!;

    [Required]

    public string Lastname { get; set; } = null!;

    [Required]

    public string Email { get; set; } = null!;

    [Required]

    public string Password { get; set; } = null!;

    [Required]

    public string ConfirmedPassword { get; set; } = null!;

}
