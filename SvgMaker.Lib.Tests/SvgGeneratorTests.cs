using JetBrains.Annotations;
using SvgMaker.Lib;
using Xunit;
using System.Text.RegularExpressions;
namespace SvgMaker.Lib.Tests;

public class SvgGeneratorTests
{
    [Fact]
    public void Instance_ReturnsSameInstance_WhenCalledMultipleTimes()
    {
        // Arrange & Act
        var instance1 = SvgGenerator.Instance;
        var instance2 = SvgGenerator.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Instance_IsNotNull()
    {
        // Arrange & Act
        var instance = SvgGenerator.Instance;

        // Assert
        Assert.NotNull(instance);
    }

    [Theory]
    [InlineData(200, 200)]
    [InlineData(500, 300)]
    [InlineData(100, 100)]
    public void GenerateRandomSvg_ReturnsSvgString_WithCorrectDimensions(int width, int height)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateRandomSvg(width, height);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"width='{width}'", result);
        Assert.Contains($"height='{height}'", result);
        Assert.StartsWith("<svg", result);
        Assert.EndsWith("</svg>", result);
    }

    [Fact]
    public void GenerateRandomSvg_ContainsCirclesAndRectangles()
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateRandomSvg(200, 200);

        // Assert
        Assert.Contains("<circle", result);
        Assert.Contains("<rect", result);
    }

    [Theory]
    [InlineData(5, 200, 200)]
    [InlineData(10, 300, 300)]
    [InlineData(1, 100, 100)]
    public void GenerateRandomRectanglesSvg_ReturnsSvgWithCorrectCount(int count, int width, int height)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateRandomRectanglesSvg(count, width, height);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"width='{width}'", result);
        Assert.Contains($"height='{height}'", result);
        var rectCount = Regex.Matches(result, "<rect").Count;
        Assert.Equal(count, rectCount);
    }

    [Theory]
    [InlineData(3, 200, 200)]
    [InlineData(8, 400, 400)]
    [InlineData(1, 150, 150)]
    public void GenerateCirclesSvg_ReturnsSvgWithCorrectCount(int count, int width, int height)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateCirclesSvg(count, width, height);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"width='{width}'", result);
        Assert.Contains($"height='{height}'", result);
        var circleCount = Regex.Matches(result, "<circle").Count;
        Assert.Equal(count, circleCount);
    }

    [Theory]
    [InlineData(200, 200)]
    [InlineData(300, 300)]
    [InlineData(100, 100)]
    public void GenerateCircleSvg_WithWidthHeight_ReturnsSvgWithCircle(int width, int height)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateCircleSvg(width, height);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"width='{width}'", result);
        Assert.Contains($"height='{height}'", result);
        Assert.Contains($"cx='{width / 2}'", result);
        Assert.Contains($"cy='{height / 2}'", result);
        Assert.Contains($"r='{width / 2}'", result);
        Assert.Matches(@"fill='#[0-9A-F]{6}'", result);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    public void GenerateCircleSvg_WithRadius_ReturnsSvgWithCorrectDimensions(int radius)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateCircleSvg(radius);

        // Assert
        Assert.NotNull(result);
        Assert.Contains($"width='{radius * 2}'", result);
        Assert.Contains($"height='{radius * 2}'", result);
        Assert.Contains($"cx='{radius}'", result);
        Assert.Contains($"cy='{radius}'", result);
        Assert.Contains($"r='{radius}'", result);
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("Test SVG")]
    [InlineData("")]
    public void GenerateSvgText_ReturnsSvgWithText(string inputText)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateSvgText(inputText);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<svg", result);
        Assert.Contains("</svg>", result);
        Assert.Contains("<text", result);
        Assert.Contains(inputText, result);
        Assert.Contains("lightgreen", result);
    }

    [Fact]
    public void GenerateSvgText_EscapesSpecialCharacters()
    {
        // Arrange
        var generator = SvgGenerator.Instance;
        var specialText = "<script>alert('xss')</script>";

        // Act
        var result = generator.GenerateSvgText(specialText);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("<script>", result);
    }

    [Theory]
    [InlineData("https://www.example.com")]
    [InlineData("https://github.com")]
    [InlineData("https://www.test.org")]
    public void GetQrSvgOfUrl_ReturnsSvgString(string url)
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GetQrSvgOfUrl(url);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<svg", result);
        Assert.Contains("</svg>", result);
    }

    [Fact]
    public void GetQrSvgOfUrl_WithCustomPixelsPerModule_ReturnsSvg()
    {
        // Arrange
        var generator = SvgGenerator.Instance;
        var url = "https://example.com";
        var pixelsPerModule = 20;

        // Act
        var result = generator.GetQrSvgOfUrl(url, pixelsPerModule);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("<svg", result);
    }

    [Fact]
    public void GenerateRandomSvg_ProducesValidSvgStructure()
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateRandomSvg(300, 300);

        // Assert
        Assert.Matches(@"<svg width='\d+' height='\d+' xmlns='http://www\.w3\.org/2000/svg'>", result);
        Assert.True(result.StartsWith("<svg"));
        Assert.True(result.EndsWith("</svg>"));
    }

    [Fact]
    public void GenerateCirclesSvg_ZeroCount_ReturnsEmptySvg()
    {
        // Arrange
        var generator = SvgGenerator.Instance;

        // Act
        var result = generator.GenerateCirclesSvg(0, 200, 200);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("<circle", result);
        Assert.Contains("<svg", result);
        Assert.Contains("</svg>", result);
    }
}
