using System;
using System.Collections.Generic;
using System.Text;
using QRCoder;

namespace SvgMaker.Lib;

public class SvgGenerator
{
    // Thread-safe, lazy initialization for the Singleton
    private static readonly Lazy<SvgGenerator> _instance = new(() => new SvgGenerator());
    public static SvgGenerator Instance => _instance.Value;

    private SvgGenerator() { }

    // ==========================================
    // PUBLIC API (Preserved for your Main class)
    // ==========================================

    public string GenerateRandomSvg(int width, int height)
    {
        return BuildSvg(width, height, svg =>
        {
            int numShapes = Random.Shared.Next(5, 11);
            for (int i = 0; i < numShapes; i++)
            {
                if (i % 2 == 0) AppendRandomCircleToSvg(svg, width, height);
                else AppendRandomRectangleToSvg(svg, width, height);
            }
        });
    }

    public string GenerateRectangleSvg(int width, int height)
    {
        return BuildSvg(width, height, svg =>
        {
            int currentX = 0, currentY = 0, squareLength = 75;
            AppendRectangleToSvg(svg, width, height, currentX, currentY);
            
            do
            {
                while (currentX < width)
                {
                    AppendRectangleToSvg(svg, squareLength, squareLength, currentX, currentY);
                    currentX += squareLength;
                }
                currentX = 0;
                currentY += squareLength;
            } while (currentY < height);
        });
    }

    public string GenerateRandomRectanglesSvg(int count, int width, int height)
    {
        return BuildSvg(width, height, svg =>
        {
            for (int i = 0; i < count; i++)
                AppendRandomRectangleToSvg(svg, width, height);
        });
    }

    public string GenerateCirclesSvg(int count, int width, int height)
    {
        return BuildSvg(width, height, svg =>
        {
            for (int i = 0; i < count; i++)
                AppendRandomCircleToSvg(svg, width, height);
        });
    }

    public string GenerateCircleSvg(int width, int height)
    {
        return BuildSvg(width, height, svg =>
        {
            svg.Append($"<circle cx='{width / 2}' cy='{height / 2}' r='{width / 2}' fill='{GetRandomColor()}' fill-opacity='1' />");
        });
    }

    public string GenerateCircleSvg(int radius)
    {
        return BuildSvg(radius * 2, radius * 2, svg =>
        {
            svg.Append($"<circle cx='{radius}' cy='{radius}' r='{radius}' fill='{GetRandomColor()}' fill-opacity='1' />");
        });
    }

    public string GenerateSvgText(string inputText)
    {
        int width = 300;
        int height = 100;

        return BuildSvg(width, height, svg =>
        {
            svg.AppendLine($"<rect x='0' y='0' width='{width}' height='{height}' fill='lightgreen' />");
            svg.AppendLine($@"<text x=""10"" y=""30"" font-family=""Arial"" font-size=""24"" fill=""black"">");
            svg.AppendLine($"    {System.Security.SecurityElement.Escape(inputText)}");
            svg.AppendLine("</text>");
        });
    }

    public string GetQrSvgOfUrl(string givenUrl, int pixelsPerModule = 10)
    {
        // Wrap IDisposable in a using statement to prevent memory leaks
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(givenUrl, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new SvgQRCode(qrCodeData);
        return qrCode.GetGraphic(pixelsPerModule);
    }

    public string GeneratePolygonSvg(int sides)
    {
        ValidateSides(sides);
        return BuildSvgWithViewBox("0 0 400 400", svg =>
        {
            string points = GetPolygonPoints(sides, 100, 150, 150);
            svg.AppendLine($@"<polygon points=""{points}"" fill=""#34dba8"" stroke=""#2c5500"" stroke-width=""2""/>");
        });
    }

    public string GenerateOnionCirclesSvg(int maxRadius)
    {
        return BuildSvg(maxRadius * 20, maxRadius * 20, svg =>
        {
            int decrementingRadius = maxRadius;
            while (decrementingRadius >= 10)
            {
                AppendCircleToSvg(svg, decrementingRadius, maxRadius + 5, maxRadius + 3);
                decrementingRadius -= 10;
            }
        });
    }

    public string GenerateOnionCirclesSvg(int layers, int maxRadius)
    {
        return BuildSvg(maxRadius * 20, maxRadius * 20, svg =>
        {
            // Fixed potential divide-by-zero/infinite loop if layers > maxRadius
            int decrement = Math.Max(1, maxRadius >= layers ? maxRadius / layers : layers / maxRadius);
            int decrementingRadius = maxRadius;

            while (decrementingRadius > 0)
            {
                AppendCircleToSvg(svg, decrementingRadius, maxRadius + 5, maxRadius + 3);
                decrementingRadius -= decrement;
            }
        });
    }

    public string GeneratePolygonsSvg(int count, int sides, int radius)
    {
        ValidateSides(sides);
        return BuildSvgWithViewBox("0 0 400 400", svg =>
        {
            for (int i = 0; i < count; i++)
            {
                AppendPolygonToSvg(svg, sides, radius, i * 10 * (i % 2 == 0 ? 1 : -1), i * (radius / 2));
            }
        });
    }

    public string GeneratePolygonSvg(int sides, int radius)
    {
        ValidateSides(sides);
        return BuildSvgWithViewBox("0 0 400 400", svg =>
        {
            string points = GetPolygonPoints(sides, radius, radius / 2, radius + 5);
            svg.AppendLine($@"<polygon points=""{points}"" fill=""{GetRandomColor()}"" stroke=""#888"" stroke-width=""2""/>");
        });
    }

    // ==========================================
    // PRIVATE HELPER METHODS (Streamlined)
    // ==========================================

    private string BuildSvg(int width, int height, Action<StringBuilder> contentBuilder)
    {
        var svg = new StringBuilder();
        svg.AppendLine($@"<svg width=""{width}"" height=""{height}"" xmlns=""http://www.w3.org/2000/svg"">");
        contentBuilder(svg);
        svg.AppendLine("</svg>");
        return svg.ToString();
    }

    private string BuildSvgWithViewBox(string viewBox, Action<StringBuilder> contentBuilder)
    {
        var svg = new StringBuilder();
        svg.AppendLine($@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""{viewBox}"">");
        contentBuilder(svg);
        svg.AppendLine("</svg>");
        return svg.ToString();
    }

    private string GetRandomColor() => $"#{Random.Shared.Next(0x1000000):X6}";

    private void ValidateSides(int sides)
    {
        if (sides < 3 || sides > 20)
            throw new ArgumentException("Sides must be between 3 and 20.", nameof(sides));
    }

    private string GetPolygonPoints(int sides, int radius, int centerX, int centerY)
    {
        var points = new List<string>(sides);
        double angleStep = 360.0 / sides;
        for (int i = 0; i < sides; i++)
        {
            double angle = i * angleStep * Math.PI / 180.0;
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY + radius * Math.Sin(angle);
            points.Add($"{x:F2},{y:F2}");
        }
        return string.Join(" ", points);
    }

    private void AppendRandomRectangleToSvg(StringBuilder svg, int width, int height)
    {
        int rX = Random.Shared.Next(0, width - 50);
        int rY = Random.Shared.Next(0, height - 50);
        int rW = Random.Shared.Next(20, 100);
        int rH = Random.Shared.Next(20, 100);
        svg.AppendLine($"<rect x='{rX}' y='{rY}' width='{rW}' height='{rH}' fill='{GetRandomColor()}' />");
    }

    private void AppendRectangleToSvg(StringBuilder svg, int width, int height, int x, int y)
    {
        svg.AppendLine($"<rect x='{x}' y='{y}' width='{width}' height='{height}' fill='{GetRandomColor()}' />");
    }

    private void AppendRandomCircleToSvg(StringBuilder svg, int width, int height)
    {
        int cx = Random.Shared.Next(0, width);
        int cy = Random.Shared.Next(0, height);
        int r = Random.Shared.Next(10, 50);
        double opacity = Random.Shared.Next(30, 100) / 100.0;
        svg.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{r}' fill='{GetRandomColor()}' fill-opacity='{opacity}' />");
    }

    private void AppendCircleToSvg(StringBuilder svg, int radius, int cx, int cy)
    {
        svg.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{radius}' fill='{GetRandomColor()}' fill-opacity='.5' />");
    }

    private void AppendPolygonToSvg(StringBuilder svg, int sides, int radius, int displacementX = 0, int displacementY = 0)
    {
        int centerX = (radius / 2) + displacementX;
        int centerY = radius + displacementY;
        string points = GetPolygonPoints(sides, radius, centerX, centerY);
        
        svg.AppendLine($@"<polygon points=""{points}"" fill=""{GetRandomColor()}"" stroke=""#222"" stroke-width=""2""/>");
    }
}