namespace AssetGen.DataStructures;

internal readonly struct GeneratedFile
{
    public readonly string Directory;

    public readonly string Contents;

    public GeneratedFile(string directory, string contents)
    {
        Directory = directory;
        Contents = contents;
    }
}
