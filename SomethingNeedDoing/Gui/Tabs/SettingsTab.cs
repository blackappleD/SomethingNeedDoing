using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using SomethingNeedDoing.Gui.Modals;

namespace SomethingNeedDoing.Gui.Tabs;

public static class SettingsTab
{
    public static void DrawTab()
    {
        using var _ = ImRaii.Child("SettingsTab", Vector2.Create(-1), false);

        ImGuiUtils.Section(Loc.Get("Settings.General"), () =>
        {
            DrawLanguageSelector();

            var chatChannel = C.ChatType;
            if (ImGuiEx.EnumCombo($"{Loc.Get("Settings.ChatType")}###ChatType", ref chatChannel))
            {
                C.ChatType = chatChannel;
                C.Save();
            }

            var errorChannel = C.ErrorChatType;
            if (ImGuiEx.EnumCombo($"{Loc.Get("Settings.ErrorChatType")}###ErrorChatType", ref errorChannel))
            {
                C.ErrorChatType = errorChannel;
                C.Save();
            }

            var propagatePause = C.PropagateControlsToChildren;
            if (ImGui.Checkbox($"{Loc.Get("Settings.PropagateControls")}###PropagateControls", ref propagatePause))
            {
                C.PropagateControlsToChildren = propagatePause;
                C.Save();
            }
            ImGuiEx.Tooltip(Loc.Get("Settings.PropagateControls.Tooltip"));

            var autoOpenStatusWindow = C.AutoOpenStatusWindow;
            if (ImGui.Checkbox($"{Loc.Get("Settings.AutoOpenStatusWindow")}###AutoOpenStatusWindow", ref autoOpenStatusWindow))
            {
                C.AutoOpenStatusWindow = autoOpenStatusWindow;
                C.Save();
            }
            ImGuiEx.Tooltip(Loc.Get("Settings.AutoOpenStatusWindow.Tooltip"));
        });

        ImGuiUtils.Section(Loc.Get("Settings.Crafting"), () =>
        {
            var craftSkip = C.CraftSkip;
            if (ImGui.Checkbox($"{Loc.Get("Settings.CraftSkip")}###CraftSkip", ref craftSkip))
            {
                C.CraftSkip = craftSkip;
                C.Save();
            }

            var smartWait = C.SmartWait;
            if (ImGui.Checkbox($"{Loc.Get("Settings.SmartWait")}###SmartWait", ref smartWait))
            {
                C.SmartWait = smartWait;
                C.Save();
            }

            var qualitySkip = C.QualitySkip;
            if (ImGui.Checkbox($"{Loc.Get("Settings.QualitySkip")}###QualitySkip", ref qualitySkip))
            {
                C.QualitySkip = qualitySkip;
                C.Save();
            }

            var loopTotal = C.LoopTotal;
            if (ImGui.Checkbox($"{Loc.Get("Settings.LoopTotal")}###LoopTotal", ref loopTotal))
            {
                C.LoopTotal = loopTotal;
                C.Save();
            }

            var loopEcho = C.LoopEcho;
            if (ImGui.Checkbox($"{Loc.Get("Settings.LoopEcho")}###LoopEcho", ref loopEcho))
            {
                C.LoopEcho = loopEcho;
                C.Save();
            }

            var useCraftLoopTemplate = C.UseCraftLoopTemplate;
            if (ImGui.Checkbox($"{Loc.Get("Settings.UseCraftLoopTemplate")}###UseCraftLoopTemplate", ref useCraftLoopTemplate))
            {
                C.UseCraftLoopTemplate = useCraftLoopTemplate;
                C.Save();
            }

            if (useCraftLoopTemplate)
            {
                var craftLoopTemplate = C.CraftLoopTemplate;
                if (ImGui.InputTextMultiline($"{Loc.Get("Settings.CraftLoopTemplate")}###CraftLoopTemplate", ref craftLoopTemplate, 1000, new Vector2(0, 100)))
                {
                    C.CraftLoopTemplate = craftLoopTemplate;
                    C.Save();
                }

                var craftLoopFromRecipeNote = C.CraftLoopFromRecipeNote;
                if (ImGui.Checkbox($"{Loc.Get("Settings.CraftLoopFromRecipeNote")}###CraftLoopFromRecipeNote", ref craftLoopFromRecipeNote))
                {
                    C.CraftLoopFromRecipeNote = craftLoopFromRecipeNote;
                    C.Save();
                }

                var craftLoopMaxWait = C.CraftLoopMaxWait;
                if (ImGui.SliderInt($"{Loc.Get("Settings.CraftLoopMaxWait")}###CraftLoopMaxWait", ref craftLoopMaxWait, 1, 10))
                {
                    C.CraftLoopMaxWait = craftLoopMaxWait;
                    C.Save();
                }

                var craftLoopEcho = C.CraftLoopEcho;
                if (ImGui.Checkbox($"{Loc.Get("Settings.CraftLoopEcho")}###CraftLoopEcho", ref craftLoopEcho))
                {
                    C.CraftLoopEcho = craftLoopEcho;
                    C.Save();
                }
            }
        });

        ImGuiUtils.Section(Loc.Get("Settings.ErrorHandling"), () =>
        {
            var stopOnError = C.StopOnError;
            if (ImGui.Checkbox($"{Loc.Get("Settings.StopOnError")}###StopOnError", ref stopOnError))
            {
                C.StopOnError = stopOnError;
                C.Save();
            }
            ImGuiEx.Tooltip(Loc.Get("Settings.StopOnError.Tooltip"));

            var maxTimeoutRetries = C.MaxTimeoutRetries;
            if (ImGui.SliderInt($"{Loc.Get("Settings.MaxTimeoutRetries")}###MaxTimeoutRetries", ref maxTimeoutRetries, 0, 10))
            {
                C.MaxTimeoutRetries = maxTimeoutRetries;
                C.Save();
            }

            var noisyErrors = C.NoisyErrors;
            if (ImGui.Checkbox($"{Loc.Get("Settings.NoisyErrors")}###NoisyErrors", ref noisyErrors))
            {
                C.NoisyErrors = noisyErrors;
                C.Save();
            }

            if (noisyErrors)
            {
                var beepFrequency = C.BeepFrequency;
                if (ImGui.SliderInt($"{Loc.Get("Settings.BeepFrequency")}###BeepFrequency", ref beepFrequency, 0, 1000))
                {
                    C.BeepFrequency = beepFrequency;
                    C.Save();
                }

                var beepDuration = C.BeepDuration;
                if (ImGui.SliderInt($"{Loc.Get("Settings.BeepDuration")}###BeepDuration", ref beepDuration, 0, 1000))
                {
                    C.BeepDuration = beepDuration;
                    C.Save();
                }

                var beepCount = C.BeepCount;
                if (ImGui.SliderInt($"{Loc.Get("Settings.BeepCount")}###BeepCount", ref beepCount, 0, 10))
                {
                    C.BeepCount = beepCount;
                    C.Save();
                }
            }
        });

        ImGuiUtils.Section(Loc.Get("Settings.Lua"), () =>
        {
            ImGui.TextWrapped(Loc.Get("Settings.LuaRequirePaths"));

            var paths = C.LuaRequirePaths.ToArray();
            using (ImRaii.Table("LuaRequirePaths", 2, ImGuiTableFlags.SizingStretchProp))
            {
                for (var index = 0; index < paths.Length; index++)
                {
                    var path = PathHelper.NormalizePath(paths[index]);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    var isValid = PathHelper.ValidatePath(path);
                    ImGui.TextColored(isValid ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed, Loc.Format("Settings.PathIndex", index));
                    ImGuiEx.Tooltip(Loc.Get(isValid ? "Settings.PathValid" : "Settings.PathInvalid"));

                    ImGui.TableNextColumn();

                    if (ImGui.InputText($"##Path{index}", ref path, 200))
                    {
                        var newPaths = paths.ToList();
                        newPaths[index] = PathHelper.NormalizePath(path);
                        C.LuaRequirePaths = [.. newPaths.Where(p => !string.IsNullOrWhiteSpace(p))];
                        C.Save();
                    }
                }
            }

            if (ImGui.Button($"{Loc.Get("Settings.AddPath")}###AddPath"))
            {
                var newPaths = paths.ToList();
                newPaths.Add(string.Empty);
                C.LuaRequirePaths = [.. newPaths];
                C.Save();
            }
        });

        ImGuiUtils.Section(Loc.Get("Settings.LegacyImport"), () =>
        {
            ImGui.TextWrapped(Loc.Format("Settings.LegacyImport.Description", P.Name));
            ImGui.Spacing();

            if (ImGuiUtils.IconButton(FontAwesomeHelper.IconImport, Loc.Get("Common.Import")))
                MigrationModal.Open(ImGui.GetClipboardText());
        });
    }

    private static void DrawLanguageSelector()
    {
        var current = C.Language ?? string.Empty;
        var options = new List<(string Code, string Name)> { (string.Empty, Loc.Get("Settings.Language.FollowDalamud")) };
        options.AddRange(Loc.SupportedLanguages);

        var preview = options.FirstOrDefault(o => o.Code == current).Name ?? options[0].Name;
        ImGui.SetNextItemWidth(200);
        using (var combo = ImRaii.Combo($"{Loc.Get("Settings.Language")}###Language", preview))
        {
            if (combo)
            {
                foreach (var (code, name) in options)
                {
                    if (ImGui.Selectable(name, code == current) && code != current)
                    {
                        C.Language = code;
                        C.Save();
                        Loc.Apply();
                    }
                }
            }
        }
        ImGuiEx.Tooltip(Loc.Get("Settings.Language.Tooltip"));
    }
}
