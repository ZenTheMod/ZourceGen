# Features

## Asset Generation

Generated files will be placed into the `{YourModName}.GeneratedAssets` namespace.

An impl of `LazyAsset<T>` will be used for all asset handling within generated files, this type will implicity convert to either an `Asset<T> `or `T` if needed.

### Textures (.png/Texture2D)

Textures will be placed into a static `Textures` class in each namespace with actual textures.\
Textures can also be placed in arrays by including numbers in the file names.

Example:\
`MyCoolImage0`, `MyCoolImage1`, `MyCoolImage2`, `MyCoolImage3`

### Effects (.fxc/.xnb/Effect)

Effects will be placed in their own static class with wrapper properties for each parameter in the shader; effects will still be generated regardless of the method used to compile them, (thanks tomat.)

### Models (.obj/ObjModel)

Models will be placed into a static class much-like effects, these classes only have basic drawing methods at the moment.

> [!IMPORTANT]
> To have models load correctly you can must manually load the `ObjModelReader` class from your mod's class!
> 
> Example:
> ```cs
> public override IContentSource CreateDefaultContentSource()
> {
>     if (!Main.dedServ)
>     {
>         AddContent(new ObjModelReader());
>     }
> 
>     return base.CreateDefaultContentSource();
> }
> ```
