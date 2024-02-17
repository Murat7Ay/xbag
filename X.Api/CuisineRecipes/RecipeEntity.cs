using Redis.OM.Modeling;
using XDataAccess;

namespace X.Api.CuisineRecipes;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "RecipeEntity" })]
public class RecipeEntity : Entity<RecipeEntity>
{
    public string title { get; set; }
    public string description { get; set; }
    public string author { get; set; }
    public string source_url { get; set; }
    public string prep_time { get; set; }
    public string cook_time { get; set; }
    public string total_time { get; set; }
    public string yield { get; set; }
    public string course { get; set; }
    public string cuisine { get; set; }
    public string difficulty { get; set; }
    public List<string> cooking_methods { get; set; }
    public Rating rating { get; set; }
    public List<Ingredient> ingredients { get; set; }
    public List<Instruction> instructions { get; set; }
    public List<string> tags { get; set; }
    public string notes { get; set; }
    public NutritionalInformation nutritional_information { get; set; }
    public DietaryRestrictions dietary_restrictions { get; set; }
    public List<string> images { get; set; }
    public List<string> equipment { get; set; }
    public OptionalFields optional_fields { get; set; }
}