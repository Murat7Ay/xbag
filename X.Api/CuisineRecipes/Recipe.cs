using Redis.OM.Modeling;
using XDataAccess;

namespace X.Api.CuisineRecipes;


[Document(StorageType = StorageType.Json, IndexName = "idx:recipe", Prefixes = new[] { "recipe" })]
public class Recipe : Entity<Recipe>
{
    [Searchable]
    public string Title { get; set; } // string
    [Searchable]
    public string Description { get; set; } // string
    [Indexed(CascadeDepth = 1)]
    public Author Author { get; set; }
    public string SourceUrl { get; set; } // string
    [Indexed(Aggregatable = true)]
    public int Servings { get; set; } // integer
    [Indexed]
    public string PrepTime { get; set; } // string
    [Indexed]
    public string ActiveCookingTime { get; set; } // string
    [Indexed]
    public string TotalTime { get; set; } // string
    [Indexed]
    public CourseType Course { get; set; } // enum
    [Indexed]
    public CuisineType Cuisine { get; set; } // enum
    [Searchable]
    public List<string> CookingMethods { get; set; } // List<string>
    public string CookingTemperature { get; set; } // string
    [Indexed(CascadeDepth = 1)]
    public Rating Rating { get; set; }
    
    [Indexed(JsonPath = "$.Name")]
    [Indexed(JsonPath = "$.Notes")]
    [Indexed(JsonPath = "$.Quantity.Value")]
    [Indexed(JsonPath = "$.Quantity.Unit")]
    public Ingredient[] Ingredients { get; set; }
    [Indexed(JsonPath = "$.StepNumber")]
    [Indexed(JsonPath = "$.Instruction")]
    public InstructionStep[] Instructions { get; set; }
    [Searchable]
    public List<string> Tags { get; set; } // List<string>
    [Searchable]
    public string Notes { get; set; } // string
    [Searchable]
    public List<string> Allergies { get; set; } // List<string>
    [Indexed(CascadeDepth = 1)]
    public NutritionalInformation NutritionalInformation { get; set; }
    [Indexed(CascadeDepth = 1)]
    public DietaryRestrictions DietaryRestrictions { get; set; }
    [Searchable]
    public List<string> Images { get; set; } // List<string>
    [Searchable]
    public List<string> Equipment { get; set; } // List<string>
    [Searchable]
    public List<string> Tips { get; set; } // List<string>
    [Indexed]
    public CostType Cost { get; set; } // enum
    [Searchable]
    public List<string> SeasonalTags { get; set; } // List<string>
    [Indexed]
    public SkillLevelType SkillLevel { get; set; } // enum
    [Searchable(JsonPath = "$.Title")]
    [Indexed(JsonPath = "$.Url")]
    public RelatedRecipe[] RelatedRecipes { get; set; }
    [Indexed(JsonPath = "$.Description")]
    [Indexed(JsonPath = "$.Name")]
    public Variation[] Variations { get; set; }
    public string VideoUrl { get; set; } // string
    [Indexed(JsonPath = "$.Source")]
    [Indexed(JsonPath = "$.Rating", CascadeDepth = 1)]
    [Indexed(JsonPath = "$.Comment", CascadeDepth = 1)]
    public ExternalReview[] ExternalReviews { get; set; }
}

public class Author
{
    [Searchable]
    public string Name { get; set; } // string
    public string Url { get; set; } // string
}

public class Rating
{
    [Indexed(Aggregatable = true)]
    public float Average { get; set; } // float
    [Indexed(Aggregatable = true)]
    public int Count { get; set; } // integer
}

public class Ingredient
{
    [Searchable]
    public string Name { get; set; } // string
    [Indexed]
    public Quantity Quantity { get; set; }
    [Searchable]
    public string Notes { get; set; } // string
    [Indexed(JsonPath = "$.Name")]
    [Indexed(JsonPath = "$.Quantity.Value")]
    [Indexed(JsonPath = "$.Quantity.Unit")]
    public Substitute[] Substitutes { get; set; }
}

public class Quantity
{
    [Indexed]
    public float Value { get; set; } // float
    [Indexed]
    public UnitType Unit { get; set; } // enum
}

public class Substitute
{
    [Indexed]
    public string Name { get; set; } // string
    [Indexed]
    public Quantity Quantity { get; set; }
}

public class InstructionStep
{
    [Indexed]
    public int StepNumber { get; set; } // integer
    [Searchable]
    public string Instruction { get; set; } // string
    public Image[] Images { get; set; }
    public List<string> Videos { get; set; } // List<string>
}

public class Image
{
    public string Url { get; set; } // string
    public string Caption { get; set; } // string
}

public class NutritionalInformation
{
    [Indexed]
    public Calories Calories { get; set; }
    [Indexed]
    public Fat Fat { get; set; }
    [Indexed]
    public Carbohydrates Carbohydrates { get; set; }
    [Indexed]
    public float Protein { get; set; } // float
    [Indexed]
    public float Sodium { get; set; } // float
    [Indexed]
    public OtherNutrient[] OtherNutrients { get; set; }
}

public class Calories
{
    [Indexed]
    public float Value { get; set; } // float
    [Indexed]
    public string Unit { get; set; } // string
}

public class Fat
{
    [Indexed]
    public float Total { get; set; } // float
    [Indexed]
    public float Saturated { get; set; } // float
    [Indexed]
    public float Trans { get; set; } // float
    [Indexed]
    public float Polyunsaturated { get; set; } // float
    [Indexed]
    public float Monounsaturated { get; set; } // float
}

public class Carbohydrates
{
    [Indexed]
    public float Total { get; set; } // float
    [Indexed]
    public float Fiber { get; set; } // float
    [Indexed]
    public float Sugar { get; set; } // float
}

public class OtherNutrient
{
    [Searchable]
    public string Name { get; set; } // string
    [Indexed]
    public float Value { get; set; } // float
    [Searchable]
    public string Unit { get; set; } // string
}

public class DietaryRestrictions
{
    [Indexed]
    public bool Vegan { get; set; } // boolean
    [Indexed]
    public bool Vegetarian { get; set; } // boolean
    [Indexed]
    public bool GlutenFree { get; set; } // boolean
    [Indexed]
    public bool DairyFree { get; set; } // boolean
    [Indexed]
    public bool NutFree { get; set; } // boolean
    [Indexed]
    public bool SoyFree { get; set; } // boolean
    [Indexed]
    public bool LowCarb { get; set; } // boolean
    [Indexed]
    public bool LowFat { get; set; } // boolean
    [Indexed]
    public bool LowSodium { get; set; } // boolean
    [Indexed]
    public bool LowCalorie { get; set; } // boolean
    [Indexed]
    public bool KetoFriendly { get; set; }
    [Indexed]
    public bool DiabeticFriendly { get; set; }
    [Indexed]
    public bool PaleoFriendly { get; set; }
    [Indexed]
    public bool HeartHealthy { get; set; }
    [Indexed]
    public bool Kosher { get; set; }
    [Indexed]
    public bool Halal { get; set; }
    [Searchable]
    public List<string> OtherRestrictions { get; set; } // List<string>
}

public class RelatedRecipe
{
    [Searchable]
    public string Title { get; set; } // string
    [Indexed]
    public string Url { get; set; } // string
}

public class Variation
{
    [Searchable]
    public string Name { get; set; } // string
    [Searchable]
    public string Description { get; set; } // string
}

public class ExternalReview
{
    [Searchable]
    public string Source { get; set; } // string
    [Indexed]
    public float Rating { get; set; } // float
    public string Url { get; set; } // string
    [Searchable]
    public string Comment { get; set; } // string
}

public enum CourseType
{
    Appetizer,
    MainCourse,
    Dessert,
    Salad,
    Soup,
    Snack,
    Drink,
    Other
}

public enum CuisineType
{
    Turkish,
    Italian,
    Mexican,
    Indian,
    Chinese,
    Thai,
    Vietnamese,
    American,
    MiddleEastern,
    Mediterranean,
    Other
}

public enum UnitType
{
    Cup,
    Ounce,
    Gram,
    Teaspoon,
    Tablespoon,
    Milliliter,
    Pinch,
    Dash,
    ToTaste,
    Other
}

public enum CostType
{
    Low,
    Moderate,
    High
}

public enum SkillLevelType
{
    Beginner,
    Intermediate,
    Advanced
}