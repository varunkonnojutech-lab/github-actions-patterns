namespace GreetingService;

/// <summary>
/// Tiny, dependency-free greeting logic. It exists so the workflows in
/// .github/workflows have something real to build and test — the star of
/// this repo is the CI, not this class.
/// </summary>
public static class Greeter
{
    /// <summary>
    /// Returns a friendly greeting. Blank or whitespace-only names fall
    /// back to "world" instead of producing an awkward empty greeting.
    /// </summary>
    public static string Greet(string? name)
    {
        var clean = name?.Trim();
        return string.IsNullOrEmpty(clean)
            ? "Hello, world!"
            : $"Hello, {clean}!";
    }

    /// <summary>
    /// Returns a greeting in the requested <paramref name="language"/>.
    /// Unknown codes fall back to English rather than throwing, so a
    /// caller can pass user input straight through.
    /// </summary>
    public static string GreetIn(string? name, string language) =>
        language.ToLowerInvariant() switch
        {
            "es" => $"¡Hola, {DisplayName(name)}!",
            "fr" => $"Bonjour, {DisplayName(name)} !",
            "de" => $"Hallo, {DisplayName(name)}!",
            _ => Greet(name),
        };

    private static string DisplayName(string? name) =>
        string.IsNullOrWhiteSpace(name) ? "world" : name.Trim();
}
