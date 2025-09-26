using System.ComponentModel.DataAnnotations;
namespace Infrastructure.Dtos;

public class ChangePasswordRequest
{
	[Required(ErrorMessage = "Du måste ange ditt nuvarande lösenord.")]
	[DataType(DataType.Password)]
	public string CurrentPassword { get; set; } = null!;

	[Required(ErrorMessage = "Du måste ange ett nytt lösenord.")]
	[DataType(DataType.Password)]
	[RegularExpression(@"^(?=.*\d).{6,}$",
		ErrorMessage = "Lösenordet måste vara minst 6 tecken och innehålla minst en siffra.")]
	public string NewPassword { get; set; } = null!;

	[Required(ErrorMessage = "Bekräfta ditt nya lösenord.")]
	[DataType(DataType.Password)]
	[Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
	public string ConfirmNewPassword { get; set; } = null!;
}