using System.ComponentModel.DataAnnotations;
using SMS.Data.Validators;

namespace SMS.Data.Entities;

public class Recipe
{
    //UniqueID
    public int Id { get; set; }

    //Foreign Key to link to User
    [Required]
    public int CreatorId { get; set; } 
   
    public string CreatorUsername { get; set; } = string.Empty;

    //================Recipe Details=================//


    [Required]
    public string RecipeName { get; set; }

    [Required]
    public string RecipeDescription { get; set; }

    [Required]
    [Range(0,5)]
    public double OverallReviewRating { get; set; }

    [UrlResource]
    public string RecipePhotoUrl { get; set; }  
    [Required]
    public int Calories { get; set; }

    [Required]
    public int Servings { get; set; }

    [Required]
    public int TimeHour { get; set; }   

    public int TimeMin { get; set; }   

    [Required]
    public int Protein { get; set; }

    [Required]
    public int Carbohydrates { get; set; }

    [Required]
    public int Fats { get; set; }

    public string Ingredients { get; set; }

    public string Instructions { get; set; }

    //List of Reviews associated with the RecipeID

    public IList<Review> Reviews {get; set; } = new List<Review>();

}//Recipe
