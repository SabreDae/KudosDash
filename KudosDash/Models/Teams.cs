using KudosDash.Models.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models
{
	public class Teams
	{
		[Key]
		[DisplayName("Team ID")]
		public int TeamId { get; set; }

		[Required]
		[StringLength(30)]
		[DisplayName("Team Name")]
		public string? TeamName { get; set; }

		public virtual ICollection<AppUser>? AppUsers { get; set; }
	}
}