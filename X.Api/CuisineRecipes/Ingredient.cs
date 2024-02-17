namespace X.Api.CuisineRecipes;

public class Ingredient
{
    public string name { get; set; }
    public string quantity { get; set; }
    public string unit { get; set; }
    public string notes { get; set; }
    public List<Substitute> substitutes { get; set; }
}