using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models
	{
	public class Teams
		{
		[Key]
		public int TeamId { get; set; }

		[Required]
		public string TeamName { get; set; }
		}
	}