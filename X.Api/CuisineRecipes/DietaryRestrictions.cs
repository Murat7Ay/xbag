namespace X.Api.CuisineRecipes;

public class DietaryRestrictions
{
    public bool vegan { get; set; }
    public bool vegetarian { get; set; }
    public bool gluten_free { get; set; }
    public bool dairy_free { get; set; }
    public bool nut_free { get; set; }
    public bool soy_free { get; set; }
    public bool low_carb { get; set; }
    public bool low_fat { get; set; }
    public bool low_sodium { get; set; }
    public bool low_calorie { get; set; }
    public bool diabetic_friendly { get; set; }
    public bool keto_friendly { get; set; }
    public bool paleo_friendly { get; set; }
    public bool heart_healthy { get; set; }
    public bool kosher { get; set; }
    public bool halal { get; set; }
    public List<string> other_restrictions { get; set; }
}