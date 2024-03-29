﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KudosDash.Models
{
	public class Feedback
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("AppUser")]
		public string? Author { get; set; }

		[Required]
		[ForeignKey("AppUser")]
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

		[Required]
		[DisplayName("Manager Approved?")]
		public bool ManagerApproved { get; set; } = false;
	}
}