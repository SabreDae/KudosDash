using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models.Users
{
	public class AccountVM
	{
		public string? Id { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string? FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string? LastName { get; set; }


		[Required(ErrorMessage = "Please enter a valid email address.")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		public string? Username { get; set; }

		[Display(Name = "Team Name")]
		public int? TeamName { get; set; } // Note that the saved value is actually the ID of the team

		[HiddenInput]
		//[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? OldPassword { get; set; }

		[HiddenInput]
		//[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? NewPassword { get; set; }
		[Display(Name = "Remember Me?")]

		[HiddenInput]
		//[Required(ErrorMessage = "Please enter a password")]
		[DataType(DataType.Password)]
		public string? ConfirmNewPassword { get; set; }
	}
}