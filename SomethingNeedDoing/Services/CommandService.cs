using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using ECommons;
using Microsoft.Extensions.Hosting;
using SomethingNeedDoing.Core.Interfaces;
using SomethingNeedDoing.Gui;

namespace SomethingNeedDoing.Services;

[RegisterSingleton, AutoConstruct]
[RegisterSingleton<IHostedService>(Duplicate = DuplicateStrategy.Append, Factory = nameof(GetHostedService))]
public partial class CommandService : IHostedService
{
    public static IHostedService GetHostedService(IServiceProvider provider) => provider.GetRequiredService<CommandService>();

    public string MainCommand => "/somethingneeddoing";
    public string[] Aliases => ["/snd", "/pcraft"];

    private readonly IMacroScheduler _macroScheduler;
    private readonly WindowSystem _windowSystem;
    private SubCommand _rootCommand = null!;

    private class SubCommand(string command, string descriptionKey, Action<string> handler, bool showInHelp = true)
    {
        public string Command { get; } = command;
        public string Description => Loc.Get(descriptionKey);
        public Action<string> Handler { get; } = handler;
        public bool ShowInHelp { get; } = showInHelp;
        public List<SubCommand> SubCommands { get; } = [];
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rootCommand = new("", "Cmd.Root", _ => _windowSystem.Toggle<MainWindow>());
        var runCommand = new SubCommand("run", "Cmd.Run", HandleRunCommand);
        runCommand.SubCommands.Add(new("loop", "Cmd.RunLoop", HandleRunLoopCommand));

        var pauseCommand = new SubCommand("pause", "Cmd.Pause", HandlePauseCommand);
        pauseCommand.SubCommands.Add(new("loop", "Cmd.PauseLoop", HandlePauseLoopCommand));
        pauseCommand.SubCommands.Add(new("all", "Cmd.PauseAll", _ => HandlePauseAllCommand()));

        var resumeCommand = new SubCommand("resume", "Cmd.Resume", HandleResumeCommand);
        resumeCommand.SubCommands.Add(new("all", "Cmd.ResumeAll", _ => HandleResumeAllCommand()));

        var stopCommand = new SubCommand("stop", "Cmd.Stop", HandleStopCommand);
        stopCommand.SubCommands.Add(new("loop", "Cmd.StopLoop", HandleStopLoopCommand));
        stopCommand.SubCommands.Add(new("all", "Cmd.StopAll", _ => _macroScheduler.StopAllMacros()));

        var helpCommand = new SubCommand("help", "Cmd.Help", _ => ShowHelp(), false);
        var statusCommand = new SubCommand("status", "Cmd.Status", _ => _windowSystem.Toggle<StatusWindow>());
        var changelogCommand = new SubCommand("changelog", "Cmd.Changelog", _ => _windowSystem.Toggle<ChangelogWindow>());

        _rootCommand.SubCommands.AddRange([runCommand, pauseCommand, stopCommand, helpCommand, resumeCommand, statusCommand, changelogCommand]);

        EzCmd.Add(MainCommand, OnChatCommand, Loc.Get("Cmd.MainHelp"), displayOrder: int.MinValue);
        Aliases.ToList().ForEach(a => EzCmd.Add(a, OnChatCommand, $"{MainCommand} Alias"));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public List<(string Command, string Description)> GetCommandData()
    {
        var result = new List<(string, string)>();
        foreach (var cmd in _rootCommand.SubCommands)
        {
            result.Add((cmd.Command, cmd.Description));
            foreach (var subCmd in cmd.SubCommands)
                result.Add(($"{cmd.Command} {subCmd.Command}", subCmd.Description));
        }
        return result;
    }

    private void OnChatCommand(string command, string arguments)
    {
        arguments = arguments.Trim();

        if (arguments == string.Empty)
        {
            _rootCommand.Handler(arguments);
            return;
        }

        var parts = arguments.Split(' ', 2);
        var subCommand = parts[0].ToLowerInvariant();
        var subArgs = parts.Length > 1 ? parts[1] : string.Empty;

        var cmd = _rootCommand.SubCommands.FirstOrDefault(c => c.Command == subCommand);
        if (cmd == null)
        {
            ShowHelp();
            return;
        }

        if (subArgs == string.Empty)
        {
            cmd.Handler(subArgs);
            return;
        }

        var subParts = subArgs.Split(' ', 2);
        var subSubCommand = subParts[0].ToLowerInvariant();
        var subSubArgs = subParts.Length > 1 ? subParts[1] : string.Empty;

        if (cmd.SubCommands.FirstOrDefault(c => c.Command == subSubCommand) is { } subCmd)
            subCmd.Handler(subSubArgs);
        else
            cmd.Handler(subArgs);
    }

    private void ShowHelp()
    {
        Svc.Chat.PrintMessage(Loc.Get("Cmd.HelpHeader"));
        foreach (var cmd in _rootCommand.SubCommands.Where(c => c.ShowInHelp))
        {
            Svc.Chat.PrintMessage($"{MainCommand} {cmd.Command} - {cmd.Description}");
            foreach (var subCmd in cmd.SubCommands)
                Svc.Chat.PrintMessage($"{MainCommand} {cmd.Command} {subCmd.Command} - {subCmd.Description}");
        }
        Svc.Chat.PrintMessage($"{MainCommand} id <macro name> - {Loc.Get("Cmd.Id")}");
    }

    private void HandleRunCommand(string arguments)
    {
        var macroName = arguments.Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _ = _macroScheduler.StartMacro(macro);
    }

    private void HandleRunLoopCommand(string arguments)
    {
        var parts = arguments.Split(' ', 2);
        if (parts.Length != 2 || !int.TryParse(parts[0], out var loopCount))
        {
            Svc.Chat.PrintErrorMsg(Loc.Get("Cmd.InvalidLoop"));
            return;
        }

        var macroName = parts[1].Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _ = _macroScheduler.StartMacro(macro, loopCount);
    }

    private void HandlePauseCommand(string arguments)
    {
        var macroName = arguments.Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _macroScheduler.PauseMacro(macro.Id);
        else
            Svc.Chat.PrintErrorMsg(Loc.Format("Cmd.MacroNotFound", macroName));
    }

    private void HandlePauseLoopCommand(string arguments)
    {
        var macroName = arguments.Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _macroScheduler.PauseAtNextLoop(macro.Id);
        else
            Svc.Chat.PrintErrorMsg(Loc.Format("Cmd.MacroNotFound", macroName));
    }

    private void HandlePauseAllCommand()
    {
        foreach (var macro in _macroScheduler.GetMacros())
            _macroScheduler.PauseMacro(macro.Id);
        Svc.Chat.PrintMessage(Loc.Get("Cmd.AllPaused"));
    }

    private void HandleResumeCommand(string arguments)
    {
        var macroName = arguments.Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _macroScheduler.ResumeMacro(macro.Id);
        else
            Svc.Chat.PrintErrorMsg(Loc.Format("Cmd.MacroNotFound", macroName));
    }

    private void HandleResumeAllCommand()
    {
        foreach (var macro in _macroScheduler.GetMacros())
            _macroScheduler.ResumeMacro(macro.Id);
        Svc.Chat.PrintMessage(Loc.Get("Cmd.AllResumed"));
    }

    private void HandleStopCommand(string arguments)
    {
        var macroName = arguments.Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _macroScheduler.StopMacro(macro.Id);
        else
            Svc.Chat.PrintErrorMsg(Loc.Format("Cmd.MacroNotFound", macroName));
    }

    private void HandleStopLoopCommand(string arguments)
    {
        var macroName = arguments.Trim('"');
        if (C.GetMacroByName(macroName) is { } macro)
            _macroScheduler.StopAtNextLoop(macro.Id);
        else
            Svc.Chat.PrintErrorMsg(Loc.Format("Cmd.MacroNotFound", macroName));
    }
}
