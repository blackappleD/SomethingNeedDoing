using System.Threading;
using System.Threading.Tasks;

namespace SomethingNeedDoing.NativeMacro.Commands;
/// <summary>
/// Controls crafting loop execution with a gate count.
/// </summary>
[GenericDoc(
    "Similar to loop but used at the start of a macro with an infinite /loop at the end. Allows a certain amount of executions before stopping the macro.",
    ["gateCount"],
    ["/gate 10", "/gate 10 <echo>"]
)]
public class GateCommand(string text, int gateCount) : MacroCommandBase(text)
{
    private readonly int startingCrafts = gateCount;
    private int remainingCount = gateCount;

    /// <inheritdoc/>
    public override bool RequiresFrameworkThread => false;

    /// <inheritdoc/>
    public override async Task Execute(MacroContext context, CancellationToken token)
    {
        if (EchoModifier?.ShouldEcho == true || C.LoopEcho)
        {
            if (remainingCount == 0)
                Svc.Chat.PrintMessage(Loc.Get("Chat.NoCraftsRemaining"));
            else
            {
                Svc.Chat.PrintMessage(Loc.Format(remainingCount == 1 ? "Chat.CraftRemaining" : "Chat.CraftsRemaining", remainingCount));
            }
        }

        remainingCount--;
        await PerformWait(token);

        if (remainingCount < 0)
        {
            remainingCount = startingCrafts;
            throw new MacroGateCompleteException();
        }
    }
}
