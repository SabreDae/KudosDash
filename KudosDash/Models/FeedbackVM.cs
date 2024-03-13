using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KudosDash.Models.Users;

namespace KudosDash.Models
{
	public class FeedbackVM
	{
		public List<Feedback> feedback { get; set; }

		public List<AppUser> user { get; set; }
	}
}