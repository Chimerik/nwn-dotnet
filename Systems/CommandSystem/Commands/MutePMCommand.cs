using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMutePMCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oTarget.IsValid)
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "__BLOCK_" + ctx.oTarget.Name + "_MP") == 0)
        {
          ObjectPlugin.SetInt(ctx.oSender, "__BLOCK_" + ctx.oTarget.Name + "_MP", 1, true);
          NWScript.SendMessageToPC(ctx.oSender, "Vous bloquez désormais tous les mps de " + ctx.oTarget.Name + ". Cette commande ne fonctionne pas sur les Dms.");
        }
        else
        {
          ObjectPlugin.DeleteInt(ctx.oSender, "__BLOCK_" + ctx.oTarget.Name + "_MP");
          NWScript.SendMessageToPC(ctx.oSender, "Vous ne bloquez plus les mps de " + ctx.oTarget.Name);
        }
      }
      else
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "__BLOCK_ALL_MP") == 0)
        {
          ObjectPlugin.SetInt(ctx.oSender, "__BLOCK_ALL_MP", 1, true);
          NWScript.SendMessageToPC(ctx.oSender, "Vous bloquez désormais l'affichage global des mps. Vous recevrez cependant toujours ceux des DMs.");
        }
        else
        {
          ObjectPlugin.DeleteInt(ctx.oSender, "__BLOCK_ALL_MP");
          NWScript.SendMessageToPC(ctx.oSender, "Vous réactivez désormais l'affichage global des mps. Vous ne recevrez cependant pas ceux que vous bloqué individuellement.");
        }
      }
    }
  }
}
