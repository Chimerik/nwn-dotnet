using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteMutePMCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (ctx.oTarget != null)
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "__BLOCK_" + ctx.oTarget.Name + "_MP") == 0)
        {
          ObjectPlugin.SetInt(ctx.oSender, "__BLOCK_" + ctx.oTarget.Name + "_MP", 1, 1);
          ctx.oSender.SendServerMessage($"Vous bloquez désormais tous les mps de {ctx.oTarget.Name.ColorString(Color.WHITE)}. Cette commande ne fonctionne pas sur les Dms.", Color.BLUE);
        }
        else
        {
          ObjectPlugin.DeleteInt(ctx.oSender, "__BLOCK_" + ctx.oTarget.Name + "_MP");
          ctx.oSender.SendServerMessage($"Vous ne bloquez plus les mps de {ctx.oTarget.Name.ColorString(Color.WHITE)}", Color.BLUE);
        }
      }
      else
      {
        if (ObjectPlugin.GetInt(ctx.oSender, "__BLOCK_ALL_MP") == 0)
        {
          ObjectPlugin.SetInt(ctx.oSender, "__BLOCK_ALL_MP", 1, 1);
          ctx.oSender.SendServerMessage("Vous bloquez désormais l'affichage global des mps. Vous recevrez cependant toujours ceux des DMs.", Color.BLUE);
        }
        else
        {
          ObjectPlugin.DeleteInt(ctx.oSender, "__BLOCK_ALL_MP");
          ctx.oSender.SendServerMessage("Vous réactivez désormais l'affichage global des mps. Vous ne recevrez cependant pas ceux que vous bloqué individuellement.", Color.BLUE);
        }
      }
    }
  }
}
