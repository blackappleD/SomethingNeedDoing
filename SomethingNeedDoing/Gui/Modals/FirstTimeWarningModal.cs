using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace SomethingNeedDoing.Gui.Modals;

public static class FirstTimeWarningModal
{
    private static Vector2 Size = new(600, 600);

    public static void Close()
    {
        C.AcknowledgedLegacyWarning = true;
        ImGui.CloseCurrentPopup();
    }

    public static unsafe void DrawModal()
    {
        if (C.AcknowledgedLegacyWarning || !AgentLobby.Instance()->IsLoggedIntoZone) return;
        var isOpen = !C.AcknowledgedLegacyWarning;

        ImGui.OpenPopup($"FirstTimeWarningPopup##{nameof(FirstTimeWarningModal)}");

        ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(Size);

        using var style = ImRaii.PushStyle(ImGuiStyleVar.WindowPadding, new Vector2(15, 15));
        using var popup = ImRaii.PopupModal($"FirstTimeWarningPopup##{nameof(FirstTimeWarningModal)}", ref isOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoTitleBar);
        if (!popup) return;

        ImGui.TextWrapped(Loc.Format("Modal.FirstTime.Rewritten", P.Name));
        ImGui.BulletText(Loc.Get("Modal.FirstTime.Native"));
        ImGui.BulletText(Loc.Get("Modal.FirstTime.Lua"));
        ImGui.BulletText(Loc.Get("Modal.FirstTime.Importer"));

        ImGui.Spacing();
        ImGuiEx.TextCentered(ImGuiColors.DalamudGrey, Loc.Get("Modal.FirstTime.Once"));

        var group = new ImGuiEx.EzButtonGroup() { IsCentered = true };
        group.Add(Loc.Get("Modal.FirstTime.Acknowledge"), Close);
        group.Draw();
    }
}
