using GreetingService;

namespace GreetingService.Tests;

public class GreeterTests
{
    [Fact]
    public void Greet_ReturnsPersonalGreeting()
    {
        Assert.Equal("Hello, Varun!", Greeter.Greet("Varun"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Greet_BlankName_FallsBackToWorld(string? name)
    {
        Assert.Equal("Hello, world!", Greeter.Greet(name));
    }

    [Fact]
    public void Greet_TrimsSurroundingWhitespace()
    {
        Assert.Equal("Hello, Ada!", Greeter.Greet("  Ada  "));
    }

    [Theory]
    [InlineData("es", "¡Hola, Ada!")]
    [InlineData("fr", "Bonjour, Ada !")]
    [InlineData("de", "Hallo, Ada!")]
    public void GreetIn_KnownLanguage_UsesThatLanguage(string language, string expected)
    {
        Assert.Equal(expected, Greeter.GreetIn("Ada", language));
    }

    [Fact]
    public void GreetIn_UnknownLanguage_FallsBackToEnglish()
    {
        Assert.Equal("Hello, Ada!", Greeter.GreetIn("Ada", "xx"));
    }

    [Fact]
    public void GreetIn_BlankName_FallsBackToWorld()
    {
        Assert.Equal("¡Hola, world!", Greeter.GreetIn(null, "es"));
    }
}
