using System.ComponentModel.DataAnnotations;
namespace SMS.Data.Entities;


// Ticket Entity
public class Review
{
    //UniqueId
    public int Id { get; set; }

    //Foregin Key to link review to recipe 
    [Required]
    public int RecipeId { get; set; }

    //Foregin Key to link review to User 
    [Required]
    public int ReviewAuthorId { get; set; }

    
    public string ReviewAuthor { get; set; }

    //========Review Attributes========//

    
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string ReviewMessage { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [Required]
    [Range(0,5)]
    public int ReviewRating { get; set; }

    //recipe linked with the review
    public Recipe recipe{get; set;}
}//Review

