namespace Ramboe.Logging.Blazor.Client;

using System;

public class ColorHelper
{
    public static string DarkenHexColor(string hexColor, double percentage)
    {
        // Validate input
        if (hexColor.Length != 7 || hexColor[0] != '#')
        {
            throw new ArgumentException("Invalid hex color code");
        }

        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage should be between 0 and 100");
        }

        // Parse the hex color
        var baseColor = Convert.ToInt32(hexColor.Substring(1), 16);

        // Calculate the new color
        var darkerColor = DarkenColor(baseColor, percentage);

        // Convert back to hex format
        var newHexColor = $"#{darkerColor:X6}";

        return newHexColor;
    }

    private static int DarkenColor(int color, double percentage)
    {
        var r = (color >> 16) & 0xFF;
        var g = (color >> 8) & 0xFF;
        var b = color & 0xFF;

        r = DarkenComponent(r, percentage);
        g = DarkenComponent(g, percentage);
        b = DarkenComponent(b, percentage);

        return (r << 16) | (g << 8) | b;
    }

    private static int DarkenComponent(int component, double percentage)
    {
        var factor = 1 - (percentage / 100);

        return (int) (component * factor);
    }

    // public static string GenerateRandomColor(string hexColor)
    // {
    //     // Validate input
    //     if (hexColor.Length != 7 || hexColor[0] != '#')
    //     {
    //         throw new ArgumentException("Invalid hex color code");
    //     }
    //
    //     // Parse the hex color
    //     var baseColor = Convert.ToInt32(hexColor.Substring(1), 16);
    //
    //     // Generate a random color in the same color family
    //     var randomColor = GenerateRandomColor(baseColor);
    //
    //     // Convert back to hex format
    //     var newHexColor = $"#{randomColor:X6}";
    //
    //     return newHexColor;
    // }

    // private static int GenerateRandomColor(int baseColor)
    // {
    //     var random = new Random();
    //
    //     var r = (baseColor >> 16) & 0xFF;
    //     var g = (baseColor >> 8) & 0xFF;
    //     var b = baseColor & 0xFF;
    //
    //     // Generate random color values within the same color family
    //     var randomR = random.Next(0, r + 1);
    //     var randomG = random.Next(0, g + 1);
    //     var randomB = random.Next(0, b + 1);
    //
    //     return (randomR << 16) | (randomG << 8) | randomB;
    // }
    
      public static string GenerateRandomColor(string hexColor)
    {
        // Validate input
        if (hexColor.Length != 7 || hexColor[0] != '#')
        {
            throw new ArgumentException("Invalid hex color code");
        }

        // Parse the hex color
        int baseColor = Convert.ToInt32(hexColor.Substring(1), 16);

        // Generate a random color in the same color family
        int randomColor = GenerateRandomColor(baseColor);

        // Convert back to hex format
        string newHexColor = $"#{randomColor:X6}";

        return newHexColor;
    }

    private static int GenerateRandomColor(int baseColor)
    {
        Random random = new Random();

        // Extract RGB components
        int r = (baseColor >> 16) & 0xFF;
        int g = (baseColor >> 8) & 0xFF;
        int b = baseColor & 0xFF;

        // Calculate hue, saturation, and lightness
        double[] hsl = RGBtoHSL(r, g, b);

        // Generate random hue value
        double randomHue = random.NextDouble() * 360;

        // Set the new hue value
        hsl[0] = randomHue;

        // Convert back to RGB
        int[] rgb = HSLtoRGB(hsl[0], hsl[1], hsl[2]);

        return (rgb[0] << 16) | (rgb[1] << 8) | rgb[2];
    }

    private static double[] RGBtoHSL(int r, int g, int b)
    {
        double normalizedR = r / 255.0;
        double normalizedG = g / 255.0;
        double normalizedB = b / 255.0;

        double max = Math.Max(normalizedR, Math.Max(normalizedG, normalizedB));
        double min = Math.Min(normalizedR, Math.Min(normalizedG, normalizedB));

        double h, s, l;
        h = s = l = (max + min) / 2;

        if (max == min)
        {
            h = s = 0; // achromatic
        }
        else
        {
            double d = max - min;
            s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

            if (max == normalizedR)
                h = (normalizedG - normalizedB) / d + (normalizedG < normalizedB ? 6 : 0);
            else if (max == normalizedG)
                h = (normalizedB - normalizedR) / d + 2;
            else if (max == normalizedB)
                h = (normalizedR - normalizedG) / d + 4;

            h /= 6;
        }

        return new double[] { h * 360, s, l };
    }

    private static int[] HSLtoRGB(double h, double s, double l)
    {
        double normalizedH = h / 360.0;

        double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        double p = 2 * l - q;

        double[] rgb = new double[3];
        rgb[0] = normalizedH + 1.0 / 3.0;
        rgb[1] = normalizedH;
        rgb[2] = normalizedH - 1.0 / 3.0;

        for (int i = 0; i < 3; i++)
        {
            if (rgb[i] < 0)
                rgb[i] += 1;
            if (rgb[i] > 1)
                rgb[i] -= 1;

            if (rgb[i] < 1.0 / 6.0)
                rgb[i] = p + (q - p) * 6 * rgb[i];
            else if (rgb[i] < 0.5)
                rgb[i] = q;
            else if (rgb[i] < 2.0 / 3.0)
                rgb[i] = p + (q - p) * 6 * (2.0 / 3.0 - rgb[i]);
            else
                rgb[i] = p;
        }

        int r = (int)(rgb[0] * 255.0);
        int g = (int)(rgb[1] * 255.0);
        int b = (int)(rgb[2] * 255.0);

        return new int[] { r, g, b };
    }
}
