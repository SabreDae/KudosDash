using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Models
	{
	public class AppUser:IdentityUser
		{
		[Required]
		[PersonalData]
		public string? FirstName { get; set; }

		[Required]
		[PersonalData]
		public string? LastName { get; set; }

		[Required]
		[PersonalData]
		public string TeamName { get; set; } = "N/A";
		}
	}
