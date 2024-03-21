using KudosDash.Models.Users;

namespace KudosDash.Models
{
	public class FeedbackVM
	{
		public List<Feedback>? Feedback { get; set; }

		public List<AppUser>? User { get; set; }
	}
}