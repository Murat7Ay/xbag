namespace X.Api.CuisineRecipes;

public class Instruction
{
    public string step_number { get; set; }
    public string instruction { get; set; }
    public List<Image> images { get; set; }
    public List<string> videos { get; set; }
}