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

    public string GenerateSvgText(string inputText,int fontSize=16)
    {
        // 1. Split text into lines to handle line breaks
        var lines = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    
        // 2. Constants for Monospace font (Courier/Consolas)
        int charWidth = fontSize-6;   // Approximate width of one character in pixels
        int lineHeight = fontSize+6;  // Height of one line plus spacing
        const int padding = 20;     // Padding around the text
    
        // 3. Calculate Dimensions
        int maxChars = lines.Max(l => l.Length);
        int width = (maxChars * charWidth) + (padding * 2);
        int height = (lines.Length * lineHeight) + (padding * 2);

        return BuildSvg(width, height, svg =>
        {
            // Background Rectangle
            svg.AppendLine($"  <rect width='100%' height='100%' fill='lightgreen' stroke='#222' />");

            // Text Element
            // 'xml:space=preserve' ensures multiple spaces aren't collapsed
            svg.AppendLine($"  <text x='{padding}' y='{padding + 15}' font-family='monospace' font-size='{fontSize}' fill='#111' xml:space='preserve'>");
    
            for (int i = 0; i < lines.Length; i++)
            {
                // Using tspan for multi-line support
                svg.AppendLine($"    <tspan x='{padding}' dy='{(i == 0 ? 0 : lineHeight)}'>{lines[i]}</tspan>");
            }
            svg.AppendLine("  </text>");
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
        return BuildSvg(maxRadius * 2+5, maxRadius * 2+5, svg =>
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
        return BuildSvg(radius * 2+5, radius * 2+5, svg =>
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
        return BuildSvg(radius * 2+5, radius * 2+5, svg =>
        {
            string points = GetPolygonPoints(sides, radius, radius , radius + 5);
            svg.AppendLine($@"<polygon points=""{points}"" fill=""{GetRandomColor()}"" stroke=""#888"" stroke-width=""2""/>");
        });
    }
    public string GenerateDartBoard(double radius)
    {
        if (radius <= 0)
            throw new ArgumentException("Radius must be greater than 0", nameof(radius));

        double center = radius/2;
        double outerBull = radius * 0.05;
        double innerBull = radius * 0.02;
        double innerRing = radius * 0.35;
        double outerRing = radius * 0.42;
        double doubleRing = radius * 0.75;

        var svg = new System.Text.StringBuilder();
        
        svg.AppendLine($"<svg width=\"{radius }\" height=\"{radius }\" xmlns=\"http://www.w3.org/2000/svg\">");
        svg.AppendLine($"  <defs>");
        svg.AppendLine($"    <style>");
        svg.AppendLine($"      .dartboard-black {{ fill: #0a0a0a; }}");
        svg.AppendLine($"      .dartboard-white {{ fill: #ffffff; }}");
        svg.AppendLine($"      .dartboard-red {{ fill: #cc0000; }}");
        svg.AppendLine($"      .dartboard-bull {{ fill: #cc0000; }}");
        svg.AppendLine($"    </style>");
        svg.AppendLine($"  </defs>");

        // Outer circle background
        svg.AppendLine($"  <circle cx=\"{center}\" cy=\"{center}\" r=\"{radius}\" class=\"dartboard-black\"/>");

        // Generate 20 dart board segments
        int[] scores = { 20, 1, 18, 4, 13, 6, 10, 15, 2, 17, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5 };
        
        for (int i = 0; i < 20; i++)
        {
            double startAngle = (i * 18 - 90) * Math.PI / 180;
            double endAngle = ((i + 1) * 18 - 90) * Math.PI / 180;
            
            string color = i % 2 == 0 ? "dartboard-black" : "dartboard-white";
            
            // Outer single ring
            DrawAnnularSegment(svg, center, startAngle, endAngle, innerRing, outerRing, color);
            
            // Outer double ring
            color = i % 2 == 0 ? "dartboard-black" : "dartboard-white";
            DrawAnnularSegment(svg, center, startAngle, endAngle, doubleRing, radius, color);
            
            // Inner single ring
            color = i % 2 == 0 ? "dartboard-white" : "dartboard-black";
            DrawAnnularSegment(svg, center, startAngle, endAngle, innerBull * 2, innerRing, color);
            
            // Triple ring
            color = i % 2 == 0 ? "dartboard-red" : "dartboard-white";
            DrawAnnularSegment(svg, center, startAngle, endAngle, radius * 0.20, radius * 0.23, color);
        }

        // Outer bull (red ring)
        svg.AppendLine($"  <circle cx=\"{center}\" cy=\"{center}\" r=\"{outerBull}\" class=\"dartboard-red\"/>");
        
        // Inner bull (black center)
        svg.AppendLine($"  <circle cx=\"{center}\" cy=\"{center}\" r=\"{innerBull}\" class=\"dartboard-black\"/>");

        svg.AppendLine("</svg>");
        
        return svg.ToString();
    }

    private void DrawAnnularSegment(System.Text.StringBuilder svg, double center, 
        double startAngle, double endAngle, double innerRadius, double outerRadius, string cssClass)
    {
        double x1Inner = center + innerRadius * Math.Cos(startAngle);
        double y1Inner = center + innerRadius * Math.Sin(startAngle);
        double x2Inner = center + innerRadius * Math.Cos(endAngle);
        double y2Inner = center + innerRadius * Math.Sin(endAngle);
        
        double x1Outer = center + outerRadius * Math.Cos(startAngle);
        double y1Outer = center + outerRadius * Math.Sin(startAngle);
        double x2Outer = center + outerRadius * Math.Cos(endAngle);
        double y2Outer = center + outerRadius * Math.Sin(endAngle);
        
        int largeArc = (endAngle - startAngle) > Math.PI ? 1 : 0;
        
        string pathData = $"M {x1Outer} {y1Outer} " +
                          $"A {outerRadius} {outerRadius} 0 {largeArc} 1 {x2Outer} {y2Outer} " +
                          $"L {x2Inner} {y2Inner} " +
                          $"A {innerRadius} {innerRadius} 0 {largeArc} 0 {x1Inner} {y1Inner} Z";
        
        svg.AppendLine($"  <path d=\"{pathData}\" class=\"{cssClass}\"/>");
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