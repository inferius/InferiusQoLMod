namespace InferiusQoL.Features.Compressor;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using InferiusQoL.Logging;
using Newtonsoft.Json;

/// <summary>
/// Nacte a drzi seznam TechType items ktere nesmi byt kompresovany lisem.
/// JSON soubor je nacitany z LanguageFiles/CompressorBlacklist.json vedle DLL
/// (presneji: stejne umisteni jako language files, protoze to je prenaseno
/// deploy targetem). User muze editovat bez rebuilu.
/// </summary>
public static class CompressorBlacklist
{
    private static readonly HashSet<TechType> _blacklisted = new HashSet<TechType>();
    private static bool _loaded = false;

    /// <summary>Pocet techtypes v blacklistu (pro diagnostiku).</summary>
    public static int Count => _blacklisted.Count;

    public static void LoadFromJson()
    {
        _blacklisted.Clear();
        _loaded = true;

        var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(dllDir))
        {
            QoLLog.Error(Category.Compressor, "Blacklist: cannot determine DLL directory");
            return;
        }

        var filePath = Path.Combine(dllDir!, "Data", "CompressorBlacklist.json");
        if (!File.Exists(filePath))
        {
            QoLLog.Warning(Category.Compressor, $"Blacklist file not found: {filePath}");
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var parsed = JsonConvert.DeserializeObject<BlacklistFile>(json);
            if (parsed?.blacklist == null)
            {
                QoLLog.Warning(Category.Compressor, "Blacklist: invalid JSON structure");
                return;
            }

            int matched = 0, unknown = 0;
            foreach (var name in parsed.blacklist)
            {
                if (TechTypeExtensions.FromString(name, out var techType, ignoreCase: true))
                {
                    _blacklisted.Add(techType);
                    matched++;
                }
                else
                {
                    QoLLog.Debug(Category.Compressor, $"Blacklist: unknown TechType '{name}'");
                    unknown++;
                }
            }

            QoLLog.Info(Category.Compressor,
                $"Blacklist loaded: {matched} resolved TechTypes, {unknown} unknown entries");
        }
        catch (System.Exception ex)
        {
            QoLLog.Error(Category.Compressor, "Blacklist: failed to parse JSON", ex);
        }
    }

    public static bool IsBlacklisted(TechType tt)
    {
        if (!_loaded) return false;
        return _blacklisted.Contains(tt);
    }

    /// <summary>Runtime pridani techtype (napr. nase custom items).</summary>
    public static void Add(TechType tt)
    {
        if (tt == TechType.None) return;
        _blacklisted.Add(tt);
    }

    private sealed class BlacklistFile
    {
        public string? comment { get; set; }
        public List<string>? blacklist { get; set; }
    }
}
