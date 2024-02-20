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

		[PersonalData]
		[ForeignKey("Teams")]
		public int? TeamId { get; set; }
		public virtual Teams? Team { get; set; } = null;
		}
	}