using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AssetGen.DataStructures;
using AssetGen.Utils;

namespace AssetGen.Core.Generators;

internal sealed class Texture2DGenerator : AssetGenerator
{
    public override string[] FileExtensions => ["png"];

    protected override IEnumerable<GeneratedFile> Write(ImmutableArray<AssetFile> textures, string assemblyName)
    {
        var writer = new StringBuilder();

            // Group all textures by their abstract path.
        var groupedPaths =
            new AliasedList<string, AssetFile>(textures, i => i.Directory);

        List<GeneratedFile> outputFiles = [];

        foreach ((HashSet<string> keys, List<AssetFile> items) in groupedPaths)
        {
            string folder = keys.First();

                // TODO: Less stupid way of checking this. :sob:
            if (items.First().InRoot)
                continue;

            string outputPath = folder;

            writer.AppendLine(HEADER);

            writer.AppendLine($$$"""
using Microsoft.Xna.Framework.Graphics;

using {{{assemblyName}}}.{{{ASSET_NAMESPACE}}}.DataStructures;

namespace {{{assemblyName}}}.{{{ASSET_NAMESPACE}}}.{{{folder.Replace('/', '.')}}};

[System.Runtime.CompilerServices.CompilerGenerated]
internal static class Textures
{
    public const string PATH = "{{{assemblyName}}}/{{{folder}}}";
""");


            HashSet<string> arrays = [];

            foreach (AssetFile texture in items)
            {
                string name = texture.Name.CleanName();

                // Don't add new properties for numbered items.
                if (!arrays.Add(name))
                {
                    continue;
                }

                string assetPath = texture.AssetPath;

                string assetName = name.Capitalize();

                // Texture arrays for numbered textures.
                List<AssetFile> arrayItems = [.. items.Where(i => i.Name.CleanName() == name)];

                if (arrayItems.Count() > 1)
                {
                    // Sort the array based on the numbers in the file name.
                    string[] sortedPaths = GetSortedPaths(arrayItems);

                    // Arrays are a bit messy, unsure if this really works well.
                    writer.AppendLine($$$"""

    public static LazyAsset<Texture2D>[] {{{assetName}}} =
    [
""");

                    foreach (string path in sortedPaths)
                        writer.AppendLine($$$"""
        new LazyAsset<Texture2D>("{{{path}}}"),
""");

                    writer.AppendLine($$$"""
    ];
""");

                    continue;
                }

                writer.AppendLine($$$"""

    public static LazyAsset<Texture2D> {{{assetName}}} = new LazyAsset<Texture2D>("{{{assetPath}}}");
""");
            }

            writer.AppendLine($$$"""}""");

            outputFiles.Add(new(Path.Combine(outputPath, "Textures.g.cs"), writer.ToString()));

            writer.Clear();
        }

        return outputFiles;

        static string[] GetSortedPaths(IEnumerable<AssetFile> textures)
        {
            return
                textures.OrderBy(t =>
                {
                    string match = string.Concat(
                            Regex.Matches(t.Name, "[0-9]")
                            .OfType<Match>()
                            .Select(m => m.ToString())
                        );

                    if (int.TryParse(match, out int result))
                    {
                        return result;
                    }

                    return 0;
                })
            .Select(t => t.AssetPath).ToArray();
        }
    }
}
