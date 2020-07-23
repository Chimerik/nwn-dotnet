namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMakeAllPCInAreaHostileCommand(ChatSystem.Context ctx, Options.Result options)
    {
      int iCount = 1;
      var oPC = NWScript.GetNearestCreature(1, 1, ctx.oSender, iCount);

      while (NWScript.GetIsObjectValid(oPC))
      {
        if (NWScript.GetArea(oPC) != NWScript.GetArea(ctx.oSender))
          break;

        if (!Utils.IsPartyMember(ctx.oSender, oPC))
          NWScript.SetPCDislike(ctx.oSender, oPC);

        iCount++;
        oPC = NWScript.GetNearestCreature(1, 1, ctx.oSender, iCount);
      }
    }
  }
}
