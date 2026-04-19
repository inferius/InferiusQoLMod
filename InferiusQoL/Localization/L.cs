namespace InferiusQoL.Localization;

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using InferiusQoL.Logging;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Centralni helper pro lokalizaci. Vsechny texty modu se nacitaji z LanguageFiles/*.json.
/// Pouziti: L.Get("InferiusQoL.Key") nebo L.Get("InferiusQoL.Key", arg1, arg2).
/// </summary>
public static class L
{
    /// <summary>Prefix pro vsechny klice naseho modu (prevence kolize s vanilla klici).</summary>
    public const string Prefix = "InferiusQoL.";

    /// <summary>Cache: klic -> jestli je registrovany (pro fallback detekci).</summary>
    private static readonly HashSet<string> _registeredKeys = new HashSet<string>();

    /// <summary>Nacte vsechny JSON soubory z LanguageFiles/ vedle DLL a zaregistruje je v Subnautica Language systemu.</summary>
    public static void LoadAll()
    {
        var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (string.IsNullOrEmpty(dllDir))
        {
            QoLLog.Error(Category.Config, "Localization: cannot determine DLL directory");
            return;
        }

        var langDir = Path.Combine(dllDir!, "LanguageFiles");
        if (!Directory.Exists(langDir))
        {
            QoLLog.Warning(Category.Config, $"Localization: folder not found: {langDir}");
            return;
        }

        foreach (var file in Directory.GetFiles(langDir, "*.json"))
        {
            var language = Path.GetFileNameWithoutExtension(file);
            try
            {
                var json = File.ReadAllText(file);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (dict == null) continue;

                foreach (var kvp in dict)
                {
                    Nautilus.Handlers.LanguageHandler.SetLanguageLine(kvp.Key, kvp.Value, language);
                    _registeredKeys.Add(kvp.Key);
                }

                QoLLog.Info(Category.Config, $"Loaded {dict.Count} translations for '{language}'");
            }
            catch (System.Exception ex)
            {
                QoLLog.Error(Category.Config, $"Localization: failed to load {file}", ex);
            }
        }
    }

    /// <summary>Vrati lokalizovany text pro klic. Pokud klic neni zaregistrovany, vrati klic samotny (pro debug).</summary>
    public static string Get(string key)
    {
        if (Language.main == null)
        {
            // Pred inicializaci Language systemu (typicky v Awake nekterych pluginu).
            return key;
        }
        var translated = Language.main.Get(key);
        // Language.Get vraci klic pokud preklad chybi. Detekujeme to a logujeme v debug.
        if (translated == key && !_registeredKeys.Contains(key))
            QoLLog.Debug(Category.Config, $"Missing translation key: {key}");
        return translated;
    }

    /// <summary>Vrati lokalizovany text s formatovanim (string.Format).</summary>
    public static string Get(string key, params object[] args)
    {
        var template = Get(key);
        try
        {
            return string.Format(template, args);
        }
        catch (System.FormatException)
        {
            QoLLog.Warning(Category.Config,
                $"Localization: format error for key '{key}', template: '{template}'");
            return template;
        }
    }

    /// <summary>Je klic zaregistrovany v nekterem jazyce?</summary>
    public static bool HasKey(string key) => _registeredKeys.Contains(key);

    /// <summary>
    /// Prelozi klic pomoci Language systemu. Pokud klic neni nalezen (nebo
    /// Language.main jeste neni ready), vrati <paramref name="englishFallback"/>.
    /// Pouzit pro CraftTree tab labels, ktere se rendruji primo bez Language lookupu
    /// - musime label rozhodnout imperativne pri registraci.
    /// </summary>
    public static string GetOrFallback(string key, string englishFallback)
    {
        if (Language.main == null) return englishFallback;
        var translated = Language.main.Get(key);
        if (string.IsNullOrEmpty(translated)) return englishFallback;
        if (translated == key) return englishFallback;
        return translated;
    }
}
