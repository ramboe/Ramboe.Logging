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

    public static string GenerateSlightlyDifferentShade(string hexColor, double degreeOfDifference)
    {
        // Validate input
        if (hexColor.Length != 7 || hexColor[0] != '#')
        {
            throw new ArgumentException("Invalid hex color code");
        }

        // Parse the hex color
        var baseColor = Convert.ToInt32(hexColor.Substring(1), 16);

        // Generate a slightly different shade in the same color family
        var newColor = GenerateSlightlyDifferentShade(baseColor, degreeOfDifference);

        // Convert back to hex format
        var newHexColor = $"#{newColor:X6}";

        return newHexColor;
    }

    private static int GenerateSlightlyDifferentShade(int baseColor, double degreeOfDifference)
    {
        // Extract RGB components
        var r = (baseColor >> 16) & 0xFF;
        var g = (baseColor >> 8) & 0xFF;
        var b = baseColor & 0xFF;

        // Introduce randomization to lightness
        var random = new Random();

        var randomFactor =
            random.NextDouble() * degreeOfDifference + (1.0 - degreeOfDifference / 2);// Random factor between (1 - degreeOfDifference/2) and (1 + degreeOfDifference/2)

        var newR = (int) (r * randomFactor);
        var newG = (int) (g * randomFactor);
        var newB = (int) (b * randomFactor);

        // Ensure the values are in the valid range (0-255)
        newR = Math.Max(0, Math.Min(255, newR));
        newG = Math.Max(0, Math.Min(255, newG));
        newB = Math.Max(0, Math.Min(255, newB));

        return (newR << 16) | (newG << 8) | newB;
    }
}
