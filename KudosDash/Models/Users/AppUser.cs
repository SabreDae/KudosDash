using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KudosDash.Models.Users
{
	public class AppUser : IdentityUser
	{

		[Required]
		[PersonalData]
		[Display(Name = "First Name")]
		public string? FirstName { get; set; }

		[Required]
		[PersonalData]
		[Display(Name = "Last Name")]
		public string? LastName { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime CreationTimestamp { get; set; } = DateTime.UtcNow;

		[PersonalData]
		[ForeignKey("Teams")]
		public int? TeamId { get; set; }
		public virtual Teams? Team { get; set; } = null;
	}
}