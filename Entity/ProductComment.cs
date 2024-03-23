using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KokoMija.Entity;

public class ProductComment
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; } // ID of the product that the comment is related to

    [Required]
    public string UserId { get; set; } // ID of the user who posted the comment

    [Required]
    [MaxLength(1000)] // Adjust the max length as needed
    public string Content { get; set; } // The text content of the comment

    [Required]
    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }

    // Properties for reply structure
    public int? ParentCommentId { get; set; } // ID of the parent comment (null for top-level comments)

    // Property for moderation status
    [Required]
    public CommentModerationStatus ModerationStatus { get; set; }

    // Navigation properties
    public Product Product { get; set; }

    public ICollection<ProductComment> Replies { get; set; } // Collection of replies

    public enum CommentModerationStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
