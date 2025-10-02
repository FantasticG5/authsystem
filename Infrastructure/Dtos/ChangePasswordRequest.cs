using System.ComponentModel.DataAnnotations;
namespace Infrastructure.Dtos;

public class ChangePasswordRequest
{
	[Required(ErrorMessage = "Du m�ste ange ditt nuvarande l�senord.")]
	[DataType(DataType.Password)]
	public string CurrentPassword { get; set; } = null!;

	[Required(ErrorMessage = "Du m�ste ange ett nytt l�senord.")]
	[DataType(DataType.Password)]
	[RegularExpression(@"^(?=.*\d).{6,}$",
		ErrorMessage = "L�senordet m�ste vara minst 6 tecken och inneh�lla minst en siffra.")]
	public string NewPassword { get; set; } = null!;

	[Required(ErrorMessage = "Bekr�fta ditt nya l�senord.")]
	[DataType(DataType.Password)]
	[Compare("NewPassword", ErrorMessage = "L�senorden matchar inte.")]
	public string ConfirmNewPassword { get; set; } = null!;
}