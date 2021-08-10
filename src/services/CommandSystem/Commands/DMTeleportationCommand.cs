namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDMTeleportationCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oSender.IsDM || ctx.oSender.PlayerName == "Chim")
      {
        if (ctx.oTarget != null)
          ctx.oSender.ControlledCreature.Location = ctx.oTarget.ControlledCreature.Location;
      }
    }
  }
}
