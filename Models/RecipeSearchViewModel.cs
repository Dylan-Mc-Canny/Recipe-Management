
using SMS.Data.Entities;

namespace SMS.Web.Models;

public class RecipeSearchViewModel
{
   // result set
public IList<Recipe> Recipes { get; set;} = new List<Recipe>();
// search options
public string Query { get; set; } = string.Empty;
}



