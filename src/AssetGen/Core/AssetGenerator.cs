using Microsoft.CodeAnalysis;
using AssetGen.DataStructures;
using AssetGen.Utils;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace AssetGen.Core;

internal abstract class AssetGenerator
{
    public abstract string[] FileExtensions { get; }

    public void AddSource(SourceProductionContext context, ImmutableArray<AssetFile> assets, string assemblyName)
    {
        if (assets.Length <= 0)
        {
            return;
        }

        IEnumerable<GeneratedFile> files = Write(assets, assemblyName);

        foreach (GeneratedFile file in files)
        {
            context.AddSource(file);
        }
    }

    protected virtual IEnumerable<GeneratedFile> Write(ImmutableArray<AssetFile> assets, string assemblyName) => [];
}
