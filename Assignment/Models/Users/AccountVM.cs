using System.ComponentModel.DataAnnotations;

namespace Assignment.Models.Users
{
	public class AccountVM
	{
		public string Id { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Please enter a valid email address.")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		[Required]
		public string? TeamName { get; set; }

		[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? OldPassword { get; set; }

		[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? NewPassword { get; set; }
		[Display(Name = "Remember Me?")]

		[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? ConfirmNewPassword { get; set; }
	}
}