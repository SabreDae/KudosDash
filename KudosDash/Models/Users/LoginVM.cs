using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models.Users
{
	public class LoginVM
	{
		[Required(ErrorMessage = "Please enter a valid email address.")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Display(Name = "Remember Me?")]
		public bool RememberMe { get; set; }
	}
}