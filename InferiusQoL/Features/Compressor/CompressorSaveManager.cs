namespace InferiusQoL.Features.Compressor;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using InferiusQoL.Logging;
using Newtonsoft.Json;

/// <summary>
/// Perzistentni seznam UniqueIdentifier.Id slisnutych item instance.
/// Ukladan do compressed-items.json.
///
/// Sprite rendering je resen prez SpriteSizeCache (runtime scan scene),
/// zadna separate TechType cache tu neni - vse je per-instance.
/// </summary>
public static class CompressorSaveManager
{
    private static readonly HashSet<string> _compressedIds = new HashSet<string>();
    private static bool _loaded = false;

    public static int Count => _compressedIds.Count;

    public static void Load()
    {
        _compressedIds.Clear();
        _loaded = true;

        var path = GetSavePath();
        if (path == null) return;

        if (!File.Exists(path))
        {
            QoLLog.Info(Category.Compressor, $"No compressor save found (fresh state). Path: {path}");
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var list = JsonConvert.DeserializeObject<List<string>>(json);
            if (list != null)
            {
                foreach (var id in list)
                {
                    if (!string.IsNullOrEmpty(id))
                        _compressedIds.Add(id);
                }
            }
            QoLLog.Info(Category.Compressor, $"Loaded {_compressedIds.Count} compressed instance IDs");
        }
        catch (Exception ex)
        {
            QoLLog.Error(Category.Compressor, $"Failed to load compressor save: {ex.Message}", ex);
        }
    }

    public static void Save()
    {
        if (!_loaded) return;
        var path = GetSavePath();
        if (path == null) return;

        try
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var list = new List<string>(_compressedIds);
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, json);
            QoLLog.Debug(Category.Compressor, $"Saved {list.Count} compressed IDs");
        }
        catch (Exception ex)
        {
            QoLLog.Error(Category.Compressor, $"Failed to save compressor data: {ex.Message}", ex);
        }
    }

    public static bool IsInstanceCompressed(string id)
    {
        if (!_loaded || string.IsNullOrEmpty(id)) return false;
        return _compressedIds.Contains(id);
    }

    /// <summary>Oznaci instance jako slisnutou. Vraci true pokud byla nove oznacena.</summary>
    public static bool MarkCompressed(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return _compressedIds.Add(id);
    }

    /// <summary>Odstrani marker (napr. kdyz TT je nove v blacklistu). Vraci true pokud byl odstranen.</summary>
    public static bool Remove(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        var removed = _compressedIds.Remove(id);
        if (removed) Save();
        return removed;
    }

    private static string? GetSavePath()
    {
        var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(dllDir)) return null;
        return Path.Combine(dllDir!, "compressed-items.json");
    }
}
