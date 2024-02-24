using KudosDash.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models
{
	public class Teams
	{
		[Key]
		public int TeamId { get; set; }

		[Required]
		[StringLength(30)]
		public string? TeamName { get; set; }

		public virtual ICollection<AppUser>? AppUsers { get; set; }
	}
}