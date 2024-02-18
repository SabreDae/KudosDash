using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KudosDash.Models.Users
{
	public class AppUser : IdentityUser
	{
		[Required]
		[PersonalData]
		public string? FirstName { get; set; }

		[Required]
		[PersonalData]
		public string? LastName { get; set; }

		//[Required]
		//[PersonalData]
		//[ForeignKey("Teams")]
		//public int TeamId { get; set; }
		//public Teams TeamName { get; set; }

		[Required]
		[PersonalData]
		public string Role { get; set; } = "Team Member";
		// Default role should be team member as Admin and Manager come with additional priviledges
	}
}