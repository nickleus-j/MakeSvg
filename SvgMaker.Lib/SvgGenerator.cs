using System.Text;
using QRCoder;

namespace SvgMaker.Lib;

public class SvgGenerator
{
    // Use Lazy<T> for thread-safe, lazy initialization
    private static readonly Lazy<SvgGenerator> _instance = 
        new Lazy<SvgGenerator>(() => new SvgGenerator());

    // Public property to access the single instance
    public static SvgGenerator Instance => _instance.Value;

    // 4. Make the constructor private so it cannot be instantiated externally
    private SvgGenerator() { }

    public string GenerateRandomSvg(int width, int height)
    {
        Random rand = new Random();
        StringBuilder svg = new StringBuilder();

        // SVG header
        svg.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");

        // Add 5-10 random circles
        int numShapes = rand.Next(5, 11);
        for (int i = 0; i < numShapes; i++)
        {
            if (i % 2 == 0)
            {
                AppendRandomCircleToSvg(svg, rand, width, height);
            }
            else
            {
                AppendRandomRectangleToSvg(svg, rand, width, height);
            }
        }
        svg.Append("</svg>");
        return svg.ToString();
    }
    public string GenerateRandomRectanglesSvg(int count, int width, int height)
    {
        Random rand = new Random();
        StringBuilder svg = new StringBuilder();
        
        // Start SVG container
        svg.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");

        //Add randomly placed and sized rectangles
        for (int i = 0; i < count; i++)
        {
            AppendRandomRectangleToSvg(svg, rand, width, height);
        }

        // Close SVG container
        svg.Append("</svg>");
        return svg.ToString();
    }
    public string GenerateCirclesSvg(int count,int width, int height)
    {
        Random rand = new Random();
        StringBuilder svg = new StringBuilder();

        // SVG header
        svg.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");

        for (int i = 0; i < count; i++)
        {
            AppendRandomCircleToSvg(svg, rand, width, height);
        }

        svg.Append("</svg>");
        return svg.ToString();
    }
    public string GenerateCircleSvg(int width, int height)
    {
        Random rand = new Random();
        StringBuilder svg = new StringBuilder();
        string color = $"#{rand.Next(0x1000000):X6}"; // Random hex color
        // SVG header
        svg.Append($"<svg width='{width}' height='{height}' xmlns='http://www.w3.org/2000/svg'>");
        svg.Append($"<circle cx='{width/2}' cy='{height/2}' r='{width/2}' fill='{color}' fill-opacity='{100.0}' />");
        svg.Append("</svg>");
        return svg.ToString();
    }
    public string GenerateCircleSvg(int radius)
    {
        Random rand = new Random();
        StringBuilder svg = new StringBuilder();
        string color = $"#{rand.Next(0x1000000):X6}"; // Random hex color
        // SVG header
        svg.Append($"<svg width='{radius*2}' height='{radius*2}' xmlns='http://www.w3.org/2000/svg'>");
        svg.Append($"<circle cx='{radius}' cy='{radius}' r='{radius}' fill='{color}' fill-opacity='{100.0}' />");
        svg.Append("</svg>");
        return svg.ToString();
    }
    public string GenerateSvgText(string inputText)
    {
        // Define SVG width/height and text position
        int width = 300;
        int height = 100;

        // Create the SVG string with the input text embedded
        string svgContent = $@"
        <svg xmlns=""http://www.w3.org/2000/svg"" width=""{width}"" height=""{height}"">
            <rect x='{0}' y='{0}' width='{width}' height='{height}' fill='lightgreen' />
            <text x=""10"" y=""30"" font-family=""Arial"" font-size=""24"" fill=""black"">
                {System.Security.SecurityElement.Escape(inputText)}
            </text>
        </svg>";

        return svgContent;
    }
    public string GetQrSvgOfUrl(string givenUrl,int pixelsPerModule=10)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(givenUrl, QRCodeGenerator.ECCLevel.Q);
        SvgQRCode qrCode = new SvgQRCode(qrCodeData);
        string qrCodeAsSvg = qrCode.GetGraphic(pixelsPerModule);
        return qrCodeAsSvg;
    }
    private void AppendRandomRectangleToSvg(StringBuilder svg,Random rand,int width, int height)
    {
        int rX = rand.Next(0, width - 50);
        int rY = rand.Next(0, height - 50);
        int rW = rand.Next(20, 100);
        int rH = rand.Next(20, 100);
        string color = $"#{rand.Next(0x1000000):X6}"; // Random hex color

        svg.Append($"<rect x='{rX}' y='{rY}' width='{rW}' height='{rH}' fill='{color}' />");
    }
    private void AppendRandomCircleToSvg(StringBuilder svg,Random rand,int width, int height)
    {
        int cx = rand.Next(0, width);
        int cy = rand.Next(0, height);
        int r = rand.Next(10, 50);
        string color = $"#{rand.Next(0x1000000):X6}"; // Random hex color
        int opacity = rand.Next(30, 100);
        svg.Append($"<circle cx='{cx}' cy='{cy}' r='{r}' fill='{color}' fill-opacity='{opacity/100.0}' />");
    }
}