using KudosDash.Models.Users;

namespace KudosDash.Models
	{
	public class AdminVM
		{
		public required IEnumerable<AppUser> Users { get; set; }
		public required IEnumerable<Teams> Teams { get; set; }
		public required IEnumerable<Feedback> FeedbackCol { get; set; }
		}
	}
