
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SMS.Data.Entities;
using SMS.Data.Services;
using System.Security.Cryptography;
using SMS.Web.Models;
using System.Data.Common;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http.Extensions;

namespace SMS.Web.Controllers;


public class RecipeController : BaseController
{
    private IRecipeService svc;

    public RecipeController()
    {
        svc = new RecipeServiceDb();
    }
    //================================ Recipe Managment ================================//

    //================ Index GET & POST Views================//
    //========Index page GET ========//
    public IActionResult Index(RecipeSearchViewModel search, string searchTerm, string orderBy, string direction)
    {
        // First, perform the search
        var results = svc.SearchRecipes(search.Query);

        // Then apply sorting if needed
        if (!string.IsNullOrEmpty(orderBy))
        {
            results = svc.FindRecipes(searchTerm, orderBy, direction).ToList();
        }

        // Set the final result
        search.Recipes = results;

        return View(search);
    }//Index-GET


    //================ Details GET & POST Views================//
    //======== Details page GET ========//
    public IActionResult Details(int id)
    {
        Recipe getRecipe = svc.GetRecipeUsingId(id);

        if (getRecipe is null)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(getRecipe);
    }//Details-GET


    //================ Create GET & POST Views================//
    //======== Create page GET ========//
    public IActionResult Create()
    {
        return View();
    }//Create-Get


    //======== Create page POST ========//
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Recipe r)
    {
        int currentUserId = User.GetSignedInUserId();
        string username = svc.GetUser(currentUserId).Name;

        if (svc.GetRecipeUsingRecipeName(r.RecipeName) != null)
        {
            ModelState.AddModelError(nameof(r.RecipeName), "Recipe Name already in use");
        }

        Recipe addRecipeWithUserDetails = new Recipe
        {
            RecipeName = r.RecipeName,
            RecipeDescription = r.RecipeDescription,
            OverallReviewRating = r.OverallReviewRating,
            RecipePhotoUrl = r.RecipePhotoUrl,
            Calories = r.Calories,
            Servings = r.Servings,
            TimeHour = r.TimeHour,
            TimeMin = r.TimeMin,
            Protein = r.Protein,
            Carbohydrates = r.Carbohydrates,
            Fats = r.Fats,
            Instructions = r.Instructions,
            Ingredients = r.Ingredients,
            CreatorId = currentUserId,
            CreatorUsername = username,
        };

        if (ModelState.IsValid)
        {
            Recipe newRecipe = svc.AddRecipe(addRecipeWithUserDetails);
            if (newRecipe is null)
            {
                Alert($"Recipe could not be created..", AlertType.warning);

                return RedirectToAction(nameof(Details), new { Id = newRecipe.Id });
            }

            return RedirectToAction(nameof(Details), new { Id = newRecipe.Id });
        }

        return View(addRecipeWithUserDetails);
    }//Create-POST


    //================ Index GET & POST Views================//
    //========Index page GET ========//

    public IActionResult Edit(int id)
    {
        Recipe selctedRecipe = svc.GetRecipeUsingId(id);

        if (selctedRecipe is null)
        {
            Alert($"Recipe {id} Not Found..", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(selctedRecipe);
    }//Edit-Get

    // POST /student/edit/{id}
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult Edit(int id, Recipe r)
    {
        //make sure the current name can still be used

        if (svc.GetRecipeUsingRecipeName(r.RecipeName) != null && svc.GetRecipeUsingRecipeName(r.RecipeName).Id != r.Id)
        {
            ModelState.AddModelError(nameof(r.RecipeName), "Recipe Name already in use");
        }

        if (ModelState.IsValid)
        {
            var recipe = svc.UpdateRecipe(r);
            if (recipe is null)
            {
                Alert("Recipe could not be updated..", AlertType.warning);
                return RedirectToAction(nameof(Details), new { Id = r.Id });
            }

            return RedirectToAction(nameof(Details), new { Id = r.Id });
        }

        return View(r);
    }//Edit-POST


    //================ Create GET & POST Views================//
    //======== Delete page GET ========//

    public IActionResult Delete(int id)
    {
        Recipe currentRecipe = svc.GetRecipeUsingId(id);

        if (currentRecipe is null)
        {
            Alert($"Recipe {id} could not be deleted..", AlertType.danger);
            return RedirectToAction(nameof(Index));
        }

        return View(currentRecipe);
    }//Delete-GET

    //======== Delete page Post ========//
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirm(int id)
    {
        bool isRecipeDeleted = svc.DeleteRecipe(id);
        if (isRecipeDeleted)
        {
            Alert("Recipe Deleted", AlertType.success);
        }
        else
        {
            Alert("Recipe could not be deleted", AlertType.warning);
        }

        return RedirectToAction(nameof(Index));
    }//Delete-POST


    //================================ Review Managment ================================//
    //================ CreateReview GET & POST Views================//


    //======== CreateReview page GET ========//
    public IActionResult CreateReview(int id)
    {
        Recipe selectedRecipe = svc.GetRecipeUsingId(id);

        int currentUserId = User.GetSignedInUserId();

        if (selectedRecipe.CreatorId == currentUserId)
        {
            Alert("You cannot leave a review on your own recipe", AlertType.warning);
            return RedirectToAction(nameof(Details), new { Id = selectedRecipe.Id });
        }

        if (selectedRecipe is null)
        {
            Alert("This Recipe does not exist", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }
        string author = string.Empty;
        if (currentUserId == 0)
        {
            author = "guest";
        }
        else
        {
            author = svc.GetUser(currentUserId).Name;
        }

        Review currentRecipeReview = new Review
        {
            RecipeId = id,
            ReviewAuthor = author,
        };


        return View(currentRecipeReview);
    }//CreateReview-GET

    //======== CreateReview page POST ========//
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateReview([Bind("RecipeId, ReviewMessage,ReviewRating")] Review newReview)
    {
        String user;
        var userid = User.GetSignedInUserId();
        if (userid == 0)
        {
            user = "guest";
        }
        else
        {
            user = svc.GetUser(userid).Name;
        }

        if (ModelState.IsValid)
        {
            Review review = svc.CreateReview(newReview.RecipeId, newReview.ReviewMessage, newReview.ReviewRating, user, userid);
            if (review is not null)
            {
                Alert("Review Created Successfully", AlertType.success);
            }
            else
            {
                Alert("Review could not be created", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Details), new { Id = review.RecipeId });
        }

        return View(newReview);
    }//CreateReview-POST


    //================ ReviewEdit GET & POST Views================//
    //======== ReviewEdit page GET ========//
    public IActionResult ReviewEdit(int id)
    {
        Review review = svc.GetReviewUsingId(id);

        if (review == null)
        {
            Alert("Ticket does not exist", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(review);
    }//ReviewEdit-GET


    //======== ReviewEdit page POST ========//
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ReviewEdit(int id, [Bind("Id, RecipeId,ReviewAuthor,ReviewAuthorId,ReviewMessage,ReviewRating")] Review updatedReview)
    {
        if (ModelState.IsValid)
        {
            Review editedReview = svc.UpdateReview(id, updatedReview.ReviewMessage, updatedReview.ReviewRating);

            if (editedReview is not null)
            {
                Alert("Ticket updated Successfully", AlertType.success);
            }
            else
            {
                Alert("Ticket could not be updated", AlertType.warning);
            }

            return RedirectToAction(
                nameof(Details), new { Id = editedReview.RecipeId }
            );
        }
        return View(updatedReview);
    }//ReviewEdit-POST


    //================ ReviewDelete GET & POST Views================//
    //======== ReviewDelete page GET ========//

    public IActionResult ReviewDelete(int id)
    {
        Review review = svc.GetReviewUsingId(id);

        if (review == null)
        {
            Alert("Review does not exist", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        return View(review);
    }//ReviewDeleteConfirm-GET



    //======== ReviewDeleteConfirm page POST ========//
    [HttpPost]
    public IActionResult ReviewDeleteConfirm(int id, int recipeId)
    {
        bool isDeleted = svc.DeleteReview(id);

        if (isDeleted)
        {
            Alert("Ticket deleted Successfully", AlertType.success);
        }
        else
        {
            Alert("Ticket could not be deleted", AlertType.warning);
        }

        return RedirectToAction(nameof(Details), new { Id = recipeId });
    }//ReviewDeleteConfirm-POST
}//RecipeController