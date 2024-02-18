using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Models.Users
{
	public class RegisterVM
	{
		[Required]
		[Display(Name = "First Name")]
		public string? FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string? LastName { get; set; }

		[Required]
		[Display(Name = "Team")]
		public string? TeamName { get; set; }

		[Required(ErrorMessage = "Please choose a role from the list.")]
		public string Role { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Re-enter password")]
		public string? ConfirmPassword { get; set; }

	}
}
