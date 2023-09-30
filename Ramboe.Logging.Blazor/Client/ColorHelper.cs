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

    public static string GenerateRandomColor(string hexColor)
    {
        // Validate input
        if (hexColor.Length != 7 || hexColor[0] != '#')
        {
            throw new ArgumentException("Invalid hex color code");
        }

        // Parse the hex color
        var baseColor = Convert.ToInt32(hexColor.Substring(1), 16);

        // Generate a random color in the same color family
        var randomColor = GenerateRandomColor(baseColor);

        // Convert back to hex format
        var newHexColor = $"#{randomColor:X6}";

        return newHexColor;
    }

    private static int GenerateRandomColor(int baseColor)
    {
        var random = new Random();

        var r = (baseColor >> 16) & 0xFF;
        var g = (baseColor >> 8) & 0xFF;
        var b = baseColor & 0xFF;

        // Generate random color values within the same color family
        var randomR = random.Next(0, r + 1);
        var randomG = random.Next(0, g + 1);
        var randomB = random.Next(0, b + 1);

        return (randomR << 16) | (randomG << 8) | randomB;
    }
}
