using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KudosDash.Models
{
	public class EditFeedbackVM
	{
		public int? Id { get; set; }

		[DisplayName("User")]
		public string? TargetUser { get; set; }

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