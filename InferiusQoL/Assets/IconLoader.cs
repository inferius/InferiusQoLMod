namespace InferiusQoL.Assets;

using System.IO;
using System.Reflection;
using InferiusQoL.Logging;
using Nautilus.Utility;
using UnityEngine;

/// <summary>
/// Nacita custom PNG ikony z Assets/Icons/ slozky vedle DLL. Pokud soubor neexistuje
/// nebo se nepodari nacist, vrati null a caller musi pouzit fallback (vanilla sprite).
///
/// Icons se deployuji pres csproj Target do BepInEx/plugins/InferiusQoL/Assets/Icons/.
/// </summary>
public static class IconLoader
{
    public static Sprite? Load(string fileName)
    {
        var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(dllDir)) return null;

        var path = Path.Combine(dllDir!, "Assets", "Icons", fileName);
        if (!File.Exists(path))
        {
            QoLLog.Warning(Category.Core, $"Icon not found: {fileName} (tried {path})");
            return null;
        }

        try
        {
            var texture = ImageUtils.LoadTextureFromFile(path);
            if (texture == null)
            {
                QoLLog.Warning(Category.Core, $"Icon texture failed to load: {fileName}");
                return null;
            }
            return ImageUtils.LoadSpriteFromTexture(texture);
        }
        catch (System.Exception ex)
        {
            QoLLog.Error(Category.Core, $"Icon load exception for {fileName}", ex);
            return null;
        }
    }

    /// <summary>Nacte ikonu nebo vrati vanilla sprite jako fallback.</summary>
    public static Sprite LoadOrFallback(string fileName, TechType fallback)
    {
        var s = Load(fileName);
        return s ?? SpriteManager.Get(fallback);
    }
}
