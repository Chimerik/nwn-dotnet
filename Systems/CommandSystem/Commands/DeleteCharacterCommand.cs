using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDeleteCharacterCommand(ChatSystem.Context ctx, Options.Result options)
    {
      AdminPlugin.DeletePlayerCharacter(ctx.oSender, 1, $"Le personnage {ctx.oSender.Name} a été supprimé.");
    }
  }
}
