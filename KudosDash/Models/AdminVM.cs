using KudosDash.Models.Users;

namespace KudosDash.Models
	{
	public class AdminVM
		{
		public IEnumerable<AppUser> Users { get; set; }
		public IEnumerable<Teams> Teams { get; set; }
		public IEnumerable<Feedback> FeedbackCol { get; set; }
		}
	}
