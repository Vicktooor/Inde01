using UnityEngine;

public static class ColorUtils
{
    private static int HexToDec(string hex)
    {
        int dec = System.Convert.ToInt32(hex, 16);
        return dec;
    }

    private static string DecToHex(int value)
    {
        return value.ToString("X2");
    }

    private static string FloatNormalizedToHex(float value)
    {
        return DecToHex(Mathf.RoundToInt(value / 255f));
    }

    private static float HexToFloatNormalized(string hex)
    {
        return HexToDec(hex) / 255f;
    }

    public static Color GetColorFromHex(string hex)
    {
        float r = HexToFloatNormalized(hex.Substring(0, 2));
        float g = HexToFloatNormalized(hex.Substring(2, 2));
        float b = HexToFloatNormalized(hex.Substring(4, 2));
        float a = 1f;
        if (hex.Length >= 8) a = HexToFloatNormalized(hex.Substring(6, 2));
        return new Color(r, g, b, a);
    }

    public static string GetHexFromColor(Color color, bool useAlpha = false)
    {
        string r = FloatNormalizedToHex(color.r);
        string g = FloatNormalizedToHex(color.g);
        string b = FloatNormalizedToHex(color.b);
        if (useAlpha) return r + g + b + FloatNormalizedToHex(color.a);
        else return r + b + g;
    }
}
