using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteGetPublicKeyCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NWScript.SendMessageToPC(ctx.oSender, "Votre clef publique est : " + NWScript.GetPCPublicCDKey(ctx.oSender));
    }
  }
}
