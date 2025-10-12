namespace Autocomplete.API.Domain.Models;

public class AutocompleteEntry
{
    public int Id { get; set; }
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string NameNorm { get; set; } = "";
    public string? Extra { get; set; }
    public DateTime UpdatedAt { get; set; }
}
