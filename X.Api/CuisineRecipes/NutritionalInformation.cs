namespace X.Api.CuisineRecipes;

public class NutritionalInformation
{
    public string calories { get; set; }
    public Fat fat { get; set; }
    public Carbohydrates carbohydrates { get; set; }
    public string protein { get; set; }
    public string sodium { get; set; }
    public List<OtherNutrient> other_nutrients { get; set; }
}