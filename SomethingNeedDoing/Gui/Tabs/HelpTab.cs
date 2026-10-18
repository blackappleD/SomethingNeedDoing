using ECommons.ImGuiMethods;

namespace SomethingNeedDoing.Gui.Tabs;

[RegisterSingleton, AutoConstruct]
public partial class HelpTab
{
    private readonly HelpLuaTab _luaTab;
    private readonly HelpCliTab _cliTab;
    private readonly HelpCommandsTab _commandsTab;

    public void Draw()
    {
        ImGuiEx.EzTabBar("Tabs",
            ($"{Loc.Get("Help.Tab.General")}###General", HelpGeneralTab.DrawTab, null, false),
            ($"{Loc.Get("Help.Tab.Commands")}###Commands", _commandsTab.DrawTab, null, false),
            ("Lua", _luaTab.DrawTab, null, false),
            ($"{Loc.Get("Help.Tab.Cli")}###Cli", _cliTab.DrawTab, null, false),
            ($"{Loc.Get("Help.Tab.Clicks")}###Clicks", HelpClicksTab.DrawTab, null, false),
            ($"{Loc.Get("Help.Tab.Keys")}###Keys", HelpKeysTab.DrawTab, null, false));
    }
}
