using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace CgaLabs.Utils;

public static class ObjParser
{
    private static CultureInfo cultureInfo = new("en-us");

    public static GraphicsModel GetModel(string path)
    {
        if (!File.Exists(path)) throw new ArgumentException("File doesn't exist");

        var lines = File.ReadAllLines(path);
        var result = new GraphicsModel();

        foreach (var line in lines)
        {
            var notCommentLine = Regex.Replace(line, @"#.*$", "");

            if (notCommentLine.Length == 0) continue;

            var parts = notCommentLine.Split(new[] { ' ' }).Where(x => x != "").ToArray();

            switch (parts[0])
            {
                case "v":
                    result.Vertexes.Add(ParseVertex(parts));
                    break;
                case "vt":
                    throw new NotImplementedException();
                case "vn":
                    throw new NotImplementedException();
                case "f":
                    result.PolygonalIndexes.Add(ParsePolygon(parts));
                    break;
            }
        }

        result.RecalculateMax();
        return result;
    }

    private static Vector4 ParseVertex(string[] parts)
    {
        return new Vector4(
            ParseFloat(parts[1]),
            ParseFloat(parts[2]),
            ParseFloat(parts[3]),
            parts.Length > 4 ? ParseFloat(parts[4]) : 1);
    }

    private static List<Vector3> ParsePolygon(string[] parts)
    {
        var result = new List<Vector3>();

        for (var i = 1; i < parts.Length; i++)
        {
            var polygonalParts = parts[i].Split("/");

            result.Add(new Vector3(
                polygonalParts.Length > 0 && polygonalParts[0] != ""
                    ? int.Parse(polygonalParts[0])
                    : 0,
                polygonalParts.Length > 1 && polygonalParts[1] != ""
                    ? int.Parse(polygonalParts[1])
                    : 0,
                polygonalParts.Length > 2 && polygonalParts[2] != ""
                    ? int.Parse(polygonalParts[2])
                    : 0));
        }

        return result;
    }

    private static float ParseFloat(string floatString)
    {
        return float.Parse(floatString, cultureInfo);
    }
}