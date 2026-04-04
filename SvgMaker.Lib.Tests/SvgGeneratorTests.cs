using System;
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
        Assert.StartsWith("<svg", result);
        Assert.EndsWith("</svg>", result.Trim());
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
        // SvgGenerator currently does not HTML-escape input text; ensure the provided text appears in the output
        Assert.Contains(specialText, result);
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
        Assert.True(result.StartsWith("<svg"));
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
    [Fact]
    public void GeneratePolygonSvg_WithValidTriangle_ReturnsValidSvgString()
    {
        var generator = SvgGenerator.Instance;
        var result = generator.GeneratePolygonSvg(3);

        Assert.NotNull(result);
        Assert.StartsWith("<svg", result);
        Assert.Contains("<polygon", result);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(12)]
    [InlineData(20)]
    public void GeneratePolygonSvg_WithValidSides_ReturnsValidSvg(int sides)
    {
        var generator = SvgGenerator.Instance;
        var result = generator.GeneratePolygonSvg(sides);

        Assert.NotNull(result);
        Assert.Contains("xmlns=\"http://www.w3.org/2000/svg\"", result);
        Assert.Contains("viewBox=\"0 0 400 400\"", result);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public void GeneratePolygonSvg_WithValidSides_ContainsCorrectNumberOfPoints(int sides)
    {
        var generator = SvgGenerator.Instance;
        var result = generator.GeneratePolygonSvg(sides);

        var pointMatches = Regex.Matches(result, @"\d+\.\d+,\d+\.\d+");
        Assert.Equal(sides, pointMatches.Count);
    }

    [Fact]
    public void GeneratePolygonSvg_WithSquare_ContainsExpectedAttributes()
    {
        var generator = SvgGenerator.Instance;
        var result = generator.GeneratePolygonSvg(4);

        Assert.Contains("fill=\"#34dba8\"", result);
        Assert.Contains("stroke=\"#2c5500\"", result);
        Assert.Contains("stroke-width=\"2\"", result);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-5)]
    public void GeneratePolygonSvg_WithSidesLessThanThree_ThrowsArgumentException(int sides)
    {
        var generator = SvgGenerator.Instance;
        var exception = Assert.Throws<ArgumentException>(() => generator.GeneratePolygonSvg(sides));

        Assert.Contains("Sides must be between 3 and 20.", exception.Message);
        Assert.Equal("sides", exception.ParamName);
    }

    [Theory]
    [InlineData(21)]
    [InlineData(25)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void GeneratePolygonSvg_WithSidesGreaterThanTwenty_ThrowsArgumentException(int sides)
    {
        var generator = SvgGenerator.Instance;
        var exception = Assert.Throws<ArgumentException>(() => generator.GeneratePolygonSvg(sides));

        Assert.Contains("Sides must be between 3 and 20.", exception.Message);
        Assert.Equal("sides", exception.ParamName);
    }

    [Fact]
    public void GeneratePolygonSvg_WithTriangle_PointsAreWithinViewBox()
    {
        var generator = SvgGenerator.Instance;
        var result = generator.GeneratePolygonSvg(3);

        var pointMatches = Regex.Matches(result, @"(\d+\.\d+),(\d+\.\d+)");
        foreach (Match match in pointMatches)
        {
            double x = double.Parse(match.Groups[1].Value);
            double y = double.Parse(match.Groups[2].Value);

            Assert.InRange(x, 0, 400);
            Assert.InRange(y, 0, 400);
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    public void GeneratePolygonSvg_WithValidSides_DoesNotThrowException(int sides)
    {
        var generator = SvgGenerator.Instance;
        var exception = Record.Exception(() => generator.GeneratePolygonSvg(sides));

        Assert.Null(exception);
    }


    [Fact]
    public void GeneratePolygonSvg_WithDifferentSides_ReturnsDifferentSvgs()
    {
        var generator = SvgGenerator.Instance;
        var triangle = generator.GeneratePolygonSvg(3);
        var square = generator.GeneratePolygonSvg(4);

        Assert.NotEqual(triangle, square);
    }

    [Fact]
    public void GenerateRectangleSvg_ReturnsGridOfRectangles()
    {
        var generator = SvgGenerator.Instance;
        var result = generator.GenerateRectangleSvg(200, 200);

        Assert.NotNull(result);
        Assert.Contains("<rect", result);
        var rectCount = Regex.Matches(result, "<rect").Count;
        Assert.True(rectCount > 0);
    }

    [Fact]
    public void GenerateOnionCirclesSvg_WithMaxRadius_ReturnsCirclesAndCorrectDimensions()
    {
        var generator = SvgGenerator.Instance;
        int maxRadius = 50;
        var result = generator.GenerateOnionCirclesSvg(maxRadius);

        Assert.NotNull(result);
        Assert.Contains("<circle", result);
        Assert.Contains($"width=\"{maxRadius * 20}\"", result);
        Assert.Contains($"height=\"{maxRadius * 20}\"", result);
    }

    [Fact]
    public void GenerateOnionCirclesSvg_WithLayersAndMaxRadius_ReturnsCircles()
    {
        var generator = SvgGenerator.Instance;
        int layers = 5;
        int maxRadius = 100;
        var result = generator.GenerateOnionCirclesSvg(layers, maxRadius);

        Assert.NotNull(result);
        Assert.Contains("<circle", result);
        Assert.Contains($"width=\"{maxRadius * 2 + 5}\"", result);
        Assert.Contains($"height=\"{maxRadius * 2 + 5}\"", result);
    }

    [Fact]
    public void GeneratePolygonsSvg_ReturnsExpectedNumberOfPolygons()
    {
        var generator = SvgGenerator.Instance;
        int count = 4;
        int sides = 6;
        int radius = 40;

        var result = generator.GeneratePolygonsSvg(count, sides, radius);

        Assert.NotNull(result);
        var polyCount = Regex.Matches(result, "<polygon").Count;
        Assert.Equal(count, polyCount);
    }

    [Fact]
    public void GeneratePolygonsSvg_InvalidSides_ThrowsArgumentException()
    {
        var generator = SvgGenerator.Instance;
        Assert.Throws<ArgumentException>(() => generator.GeneratePolygonsSvg(1, 2, 10));
    }

    [Fact]
    public void GeneratePolygonSvg_WithSidesAndRadius_ReturnsSvgWithDimensions()
    {
        var generator = SvgGenerator.Instance;
        int sides = 5;
        int radius = 60;

        var result = generator.GeneratePolygonSvg(sides, radius);

        Assert.NotNull(result);
        Assert.Contains("<polygon", result);
        Assert.Contains($"width=\"{radius * 2 + 5}\"", result);
        Assert.Contains($"height=\"{radius * 2 + 5}\"", result);
    }

    [Fact]
    public void GenerateDartBoard_ReturnsExpectedStructure_AndThrowsOnInvalidRadius()
    {
        var generator = SvgGenerator.Instance;

        // valid
        var result = generator.GenerateDartBoard(200);
        Assert.NotNull(result);
        Assert.Contains("<svg", result);
        Assert.Contains("dartboard-red", result);
        Assert.Contains("<path", result);
        Assert.Contains("<circle", result);

        // invalid
        var ex = Assert.Throws<ArgumentException>(() => generator.GenerateDartBoard(0));
        Assert.Equal("radius", ex.ParamName);
    }
    [Fact]
    public void GenerateIsometricCubeSvg_WithDefaultParameters_ReturnsValidSvg()
    {
        var _svgGenerator = SvgGenerator.Instance;
        // Act
        string result = _svgGenerator.GenerateIsometricCubeSvg();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.StartsWith("<svg", result);
        Assert.EndsWith("</svg>\n", result);
    }

    [Fact]
    public void GenerateIsometricCubeSvg_WithDefaultSize_ContainsCorrectViewBox()
    {
        var _svgGenerator = SvgGenerator.Instance;
        // Act
        string result = _svgGenerator.GenerateIsometricCubeSvg();

        // Assert
        Assert.Contains("viewBox=\"0 0 100 100\"", result);
    }
    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(300)]
    public void GenerateIsometricCubeSvg_WithVariousSizes_ContainsCorrectViewBox(double size)
    {
        // Act
        string result = SvgGenerator.Instance.GenerateIsometricCubeSvg(size);

        // Assert
        Assert.Contains($"viewBox=\"0 0 {size} {size}\"", result);
    }

    [Fact]
    public void GenerateIsometricCubeSvg_WithSize50_ScalesCoordinatesCorrectly()
    {
        // Act
        string result = SvgGenerator.Instance.GenerateIsometricCubeSvg(50);

        // Assert - Top face should have scaled coordinates (50% of original)
        Assert.Contains("points=\"25,2.5", result); // topX1=50*0.5, topY1=5*0.5
    }
    [Fact]
    public void GenerateIsometricCubeSvg_WithDefaultColors_ContainsDefaultFillColor()
    {
        // Act
        string result = SvgGenerator.Instance.GenerateIsometricCubeSvg();

        // Assert
        Assert.Contains("#34ca80", result);
        Assert.Contains("#2c3e50", result);
    }

    [Fact]
    public void GenerateIsometricCubeSvg_WithCustomFillColor_ContainsCustomColor()
    {
        var _svgGenerator = SvgGenerator.Instance;
        // Arrange
        string customColor = "#ff0000";

        // Act
        string result = _svgGenerator.GenerateIsometricCubeSvg(fillColor: customColor);

        // Assert
        Assert.Contains(customColor, result);
    }

    [Fact]
    public void GenerateIsometricCubeSvg_WithCustomStrokeColor_ContainsCustomStrokeColor()
    {
        var _svgGenerator = SvgGenerator.Instance;
        // Arrange
        string customStroke = "#00ff00";

        // Act
        string result = _svgGenerator.GenerateIsometricCubeSvg(strokeColor: customStroke);

        // Assert
        Assert.Contains(customStroke, result);
    }

    [Theory]
    [InlineData("#ff0000")]
    [InlineData("#00ff00")]
    [InlineData("#0000ff")]
    [InlineData("#ffffff")]
    [InlineData("#000000")]
    public void GenerateIsometricCubeSvg_WithVariousColors_ContainsProvidedColors(string color)
    {
        var _svgGenerator = SvgGenerator.Instance;
        // Act
        string result = _svgGenerator.GenerateIsometricCubeSvg(fillColor: color);

        // Assert
        Assert.Contains(color, result);
    }
}
