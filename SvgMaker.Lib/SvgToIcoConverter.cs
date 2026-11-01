using ImageMagick;
namespace SvgMaker.Lib;

public class SvgToIcoConverter
{
    /// <summary>
    /// Converts an SVG string to an ICO file (byte array)
    /// </summary>
    public static byte[] SvgStringToIco(string svgString, uint size = 256)
    {
        try
        {
            using (var image = new MagickImage())
            {
                // Configure SVG rendering settings
                var settings = new MagickReadSettings
                {
                    Width = size,
                    Height = size,
                    Density = new Density(300, 300) // Higher DPI = better quality
                };

                // Read SVG from string
                image.Read(System.Text.Encoding.UTF8.GetBytes(svgString), settings);

                // Ensure correct size
                if (image.Width != size || image.Height != size)
                {
                    image.Resize(size, size);
                }

                // Set format to ICO
                image.Format = MagickFormat.Ico;

                // Return as byte array
                return image.ToByteArray();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert SVG to ICO: {ex.Message}", ex);
        }
    }
}