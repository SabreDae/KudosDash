using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models.Users
{
	public class RegisterVM
	{
		[Required]
		[Display(Name = "First Name")]
		public string? FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string? LastName { get; set; }

		[Display(Name = "Team")]
		public int? TeamId { get; set; }

		[Required]
		public string? Role { get; set; }

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