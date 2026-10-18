using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using SomethingNeedDoing.Managers;

namespace SomethingNeedDoing.Gui.Tabs;

public static class HelpGeneralTab
{
    public static void DrawTab()
    {
        using var child = ImRaii.Child(nameof(HelpGeneralTab));
        ImGuiUtils.Section(P.Name, () =>
        {
            ImGui.TextWrapped(Loc.Format("Help.General.Intro1", P.Name));
            ImGui.TextWrapped(Loc.Get("Help.General.Intro2"));
        });

        ImGuiUtils.Section(Loc.Get("Help.General.Status"), () =>
        {
            ImGui.TextWrapped(Loc.Get("Help.General.StatusDescription"));
            ImGui.Spacing();

            ImGuiEx.Text(ImGuiColors.DalamudOrange, Loc.Get("Help.General.MacroStates"));
            ImGui.BulletText(Loc.Get("Help.General.State.Ready"));
            ImGui.BulletText(Loc.Get("Help.General.State.Running"));
            ImGui.BulletText(Loc.Get("Help.General.State.Paused"));
            ImGui.BulletText(Loc.Get("Help.General.State.Completed"));
            ImGui.BulletText(Loc.Get("Help.General.State.Failed"));
        });

        ImGuiUtils.Section(Loc.Get("MacroSettings.TriggerEvents"), () =>
        {
            ImGui.TextWrapped(Loc.Get("Help.General.Triggers.Intro"));
            Enum.GetNames<TriggerEvent>().Each(name => ImGui.BulletText(name));

            ImGui.TextWrapped(Loc.Get("Help.General.Triggers.Lua"));
            ImGui.TextWrapped(Loc.Format("Help.General.Triggers.Register", nameof(TriggerEventManager)));
            ImGui.TextWrapped(Loc.Format("Help.General.Triggers.Addon", TriggerEvent.OnAddonEvent));
            ImGui.SameLine();
            ImGuiEx.Text(ImGuiColors.DalamudOrange, "OnAddonEvent_SelectYesno_PostSetup");
        });

        ImGuiUtils.Section(Loc.Get("Help.General.Metadata"), () =>
        {
            ImGuiEx.Text(ImGuiColors.DalamudOrange, Loc.Get("Help.Tab.General"));
            ImGui.TextWrapped(Loc.Get("Help.General.Metadata.1"));
            ImGui.TextWrapped(Loc.Get("Help.General.Metadata.2"));
            ImGui.TextWrapped(Loc.Get("Help.General.Metadata.3"));

            ImGuiEx.Text(ImGuiColors.DalamudOrange, Loc.Get("Help.General.Deps"));
            ImGui.TextWrapped(Loc.Get("Help.General.Deps.1"));
            ImGui.TextWrapped(Loc.Get("Help.General.Deps.2"));
            ImGui.TextWrapped(Loc.Get("Help.General.Deps.3"));
        });

        ImGuiUtils.Section(Loc.Get("Help.General.Git"), () =>
        {
            ImGui.TextWrapped(Loc.Get("Help.General.Git.1"));
            ImGui.TextWrapped(Loc.Get("Help.General.Git.2"));
            ImGui.TextWrapped(Loc.Get("Help.General.Git.3"));
        });
    }
}
