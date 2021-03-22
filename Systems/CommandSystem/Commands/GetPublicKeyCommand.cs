using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteGetPublicKeyCommand(ChatSystem.Context ctx, Options.Result options)
    {
      ctx.oSender.SendServerMessage($"Votre clef publique est : {ctx.oSender.CDKey.ColorString(Color.WHITE)}", Color.PINK);
    }
  }
}
