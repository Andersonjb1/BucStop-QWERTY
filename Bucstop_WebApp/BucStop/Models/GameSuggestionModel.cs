using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace BucStop.Models
{
    public class GameSubmissionModel
    {
        // Data from the form inputs
        public string? Username { get; set; }

        // Data parsed from the .txt file

        // Corresponds to Game.Title
        [Required]
        [StringLength(50, ErrorMessage = "Title must be under 50 characters.")]
        public string? SuggestedTitle { get; set; }

        // Corresponds to Game.Author (The suggester's name)
        public string? SuggestedAuthor { get; set; }

        // Corresponds to Game.Description
        [Required]
        [DataType(DataType.MultilineText)]
        public string? SuggestedDescription { get; set; }

        // Corresponds to Game.HowTo
        [Required]
        public string? SuggestedHowTo { get; set; }

        // HIGH RISK: The actual game code provided by the user (corresponds to Game.Content)
        public string? RawJsCodeContent { get; set; }

        // LOW RISK: A suggested thumbnail URL or filename
        public string? SuggestedThumbnailUrl { get; set; }

    }
}