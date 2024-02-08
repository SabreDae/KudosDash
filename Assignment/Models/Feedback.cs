﻿using System.ComponentModel.DataAnnotations;

namespace Assignment.Models
	{
	public class Feedback
		{
		[Key]
		public int ID { get; set; }

		[Required]
		public string Author { get; set; }

		[Required]
		public string TargetUser { get; set; }

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
