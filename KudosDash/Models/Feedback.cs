using KudosDash.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KudosDash.Models
	{
	public class Feedback
		{
		[Key]
		public int ID { get; set; }

		[Required]
		[ForeignKey("AppUser")]
		public string? Author { get; set; }

		[Required]
		[ForeignKey("AppUser")]
		public string? TargetUser { get; set; }

		public virtual AppUser? User { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date")]
		public DateTime FeedbackDate { get; set; } = DateTime.Now;

		[Required]
		[Display(Name = "Feedback")]
		[DataType(DataType.MultilineText)]
		[StringLength(500, ErrorMessage = "Please do not enter more than 500 characters!")]
		public string? FeedbackText { get; set; }
		}
	}