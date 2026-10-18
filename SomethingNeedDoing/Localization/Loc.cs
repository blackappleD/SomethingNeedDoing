using Dalamud.Plugin;
using Newtonsoft.Json;
using System.IO;

namespace SomethingNeedDoing.Localization;

/// <summary>
/// 轻量级界面字符串表。默认跟随 Dalamud 界面语言，可在设置中手动指定；
/// 缺失的键或语言回退到英文。ImGui 每帧重绘，切换语言后立即生效。
/// </summary>
public static class Loc
{
    public const string DefaultLanguage = "en";

    public static readonly IReadOnlyList<(string Code, string Name)> SupportedLanguages =
    [
        ("en", "English"),
        ("zh", "简体中文"),
    ];

    private static Dictionary<string, string> _fallback = [];
    private static Dictionary<string, string> _current = [];
    private static IDalamudPluginInterface? _pluginInterface;

    public static string CurrentLanguage { get; private set; } = DefaultLanguage;

    public static void Init(IDalamudPluginInterface pluginInterface)
    {
        _pluginInterface = pluginInterface;
        _fallback = Load(DefaultLanguage) ?? [];
        pluginInterface.LanguageChanged += OnDalamudLanguageChanged;
        Apply();
    }

    public static void Dispose()
    {
        if (_pluginInterface != null)
            _pluginInterface.LanguageChanged -= OnDalamudLanguageChanged;
        _pluginInterface = null;
    }

    private static void OnDalamudLanguageChanged(string langCode) => Apply();

    /// <summary>重新解析当前语言（配置或 Dalamud 界面语言变化后调用）。</summary>
    public static void Apply()
    {
        var code = ResolveLanguage();
        _current = code == DefaultLanguage ? _fallback : Load(code) ?? _fallback;
        CurrentLanguage = code;
    }

    public static string Get(string key)
    {
        if (_current.TryGetValue(key, out var text) || _fallback.TryGetValue(key, out text))
            return text;
        Svc.Log.Verbose($"[Loc] Missing key: {key}");
        return key;
    }

    /// <summary>返回 <paramref name="key"/> 的翻译，不存在时返回 <paramref name="fallback"/>。</summary>
    public static string GetOr(string key, string fallback)
        => _current.TryGetValue(key, out var text) || _fallback.TryGetValue(key, out text) ? text : fallback;

    public static string Format(string key, params object?[] args)
    {
        try
        {
            return string.Format(Get(key), args);
        }
        catch (FormatException)
        {
            return _fallback.TryGetValue(key, out var text) ? string.Format(text, args) : key;
        }
    }

    private static string ResolveLanguage()
    {
        var configured = C?.Language;
        var source = string.IsNullOrWhiteSpace(configured) ? _pluginInterface?.UiLanguage : configured;
        // 区域限定的代码（"zh-CN"、"zh_CN"）按基础语言处理
        var code = (source ?? DefaultLanguage).Split('-', '_')[0].ToLowerInvariant();
        return SupportedLanguages.Any(l => l.Code == code) ? code : DefaultLanguage;
    }

    private static Dictionary<string, string>? Load(string code)
    {
        try
        {
            using var stream = typeof(Loc).Assembly.GetManifestResourceStream($"SomethingNeedDoing.Localization.{code}.json");
            if (stream == null)
                return null;
            using var reader = new StreamReader(stream);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, $"[Loc] Failed to load language {code}");
            return null;
        }
    }
}
