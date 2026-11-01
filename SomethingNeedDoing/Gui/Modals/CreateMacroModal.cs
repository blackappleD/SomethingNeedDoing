using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using SomethingNeedDoing.Managers;
using System.Threading.Tasks;

namespace SomethingNeedDoing.Gui.Modals;

public class CreateMacroModal(GitMacroManager gitManager)
{
    private static Vector2 Size = new(400, 250);
    private static bool IsOpen = false;

    private static string _newMacroName = "New Macro";
    private static MacroType _newMacroType = MacroType.Native;
    private static string _githubUrl = "";
    private static bool _isImporting = false;
    private static string _importError = "";

    public void Open()
    {
        IsOpen = true;
        _newMacroName = Loc.Get("Modal.CreateMacro.DefaultName");
        _newMacroType = MacroType.Native;
        _githubUrl = "";
        _importError = "";
    }

    public void Close()
    {
        IsOpen = false;
        ImGui.CloseCurrentPopup();
    }

    public void DrawModal()
    {
        if (!IsOpen) return;

        ImGui.OpenPopup($"CreateMacroPopup##{nameof(CreateMacroModal)}");

        ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(Size);

        using var style = ImRaii.PushStyle(ImGuiStyleVar.WindowPadding, new Vector2(15, 15));
        using var popup = ImRaii.PopupModal($"CreateMacroPopup##{nameof(CreateMacroModal)}", ref IsOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoTitleBar);
        if (!popup) return;

        ImGuiEx.Icon(FontAwesomeHelper.IconNew);
        ImGui.SameLine();
        ImGui.Text(Loc.Get("Modal.CreateMacro.Title"));
        ImGui.Separator();

        ImGui.AlignTextToFramePadding();
        ImGui.Text(Loc.Get("Common.Name"));

        ImGui.SameLine();
        ImGuiUtils.SetFocusIfAppearing();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        ImGui.InputText("##MacroName", ref _newMacroName, 100);

        ImGui.Spacing();

        ImGui.AlignTextToFramePadding();
        ImGui.Text(Loc.Get("Common.Type"));
        ImGui.SameLine();

        _newMacroType = ImGuiUtils.EnumRadioButtons(_newMacroType);

        ImGui.Spacing();

        ImGuiUtils.Section(Loc.Get("Modal.CreateMacro.ImportSection"), () =>
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text(Loc.Get("Common.GitHubUrl"));
            ImGui.SameLine();
            ImGuiEx.Tooltip(Loc.Get("Common.GitHubUrl.Tooltip"));

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputText("##GitHubUrl", ref _githubUrl, 500);

            if (!string.IsNullOrEmpty(_importError))
            {
                ImGui.Spacing();
                ImGuiEx.TextWrapped(EzColor.RedBright.Vector4, _importError);
            }
        });

        ImGui.Spacing();
        ImGui.Spacing();

        if (_isImporting)
            ImGuiUtils.CenteredButtons((Loc.Get("Modal.CreateMacro.Importing"), () => { }));
        else
        {
            ImGuiUtils.CenteredButtons((Loc.Get("Common.Create"), async () =>
            {
                if (!string.IsNullOrWhiteSpace(_githubUrl))
                    await ImportFromGitHub();
                else
                {
                    var uniqueName = C.GetUniqueMacroName(_newMacroName);
                    var newMacro = new ConfigMacro
                    {
                        Name = uniqueName,
                        Type = _newMacroType == 0 ? MacroType.Native : MacroType.Lua,
                        Content = string.Empty,
                        FolderPath = ConfigMacro.Root
                    };

                    C.Macros.Add(newMacro);
                    C.Save();
                    Close();
                }
            }
            ), (Loc.Get("Common.Cancel"), Close));
        }
    }

    private async Task ImportFromGitHub()
    {
        _isImporting = true;
        _importError = "";

        try
        {
            // Create the git macro using the GitMacroManager
            var macro = await gitManager.Import(_githubUrl);

            // Update the name if provided
            if (!string.IsNullOrWhiteSpace(_newMacroName) && _newMacroName != Loc.Get("Modal.CreateMacro.DefaultName"))
                macro.Name = C.GetUniqueMacroName(_newMacroName);

            macro.Type = _newMacroType;
            C.Save();
            Close();
        }
        catch (Exception ex)
        {
            _importError = Loc.Format("Modal.CreateMacro.ImportFailed", ex.Message);
            FrameworkLogger.Error(ex, "Failed to import macro from GitHub");
        }
        finally
        {
            _isImporting = false;
        }
    }
}
