namespace X.Api.CuisineRecipes;

public class OptionalFields
{
    public string source_notes { get; set; }
    public string video_url { get; set; }
    public List<ExternalReview> external_reviews { get; set; }
    public List<string> tips { get; set; }
    public string cost { get; set; }
    public List<string> seasonalTags { get; set; }
    public string skillLevel { get; set; }
    public List<RelatedRecipe> related_recipes { get; set; }
    public List<Variation> variations { get; set; }
}