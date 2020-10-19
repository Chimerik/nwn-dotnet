using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSetRoleplayBonusCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender) == 1)
      {
        if (NWScript.GetIsObjectValid(ctx.oTarget) == 1)
        {
          if (((string)options.positional[0]).Length == 0)
          {
            NWScript.SendMessageToPC(ctx.oSender, $"Le bonus roleplay de {NWScript.GetName(ctx.oTarget)} est de {ObjectPlugin.GetInt(ctx.oTarget, "_BRP")}");
          }
          else
          {
            int iBRP;
            if (int.TryParse((string)options.positional[0], out iBRP))
            {
              if(iBRP > -1 && iBRP < 5)
              {
                ObjectPlugin.SetInt(ctx.oTarget, "_BRP", iBRP, 1); // TODO : le BRP est valable pour tout le compte du joueur, pas juste le perso. A enregistrer en BDD
                NWScript.SendMessageToPC(ctx.oSender, $"Le bonus roleplay de {NWScript.GetName(ctx.oTarget)} est de { ObjectPlugin.GetInt(ctx.oTarget, "_BRP")}");
                NWScript.SendMessageToPC(ctx.oTarget, $"Votre bonus roleplay est désormais de { ObjectPlugin.GetInt(ctx.oTarget, "_BRP")}");
              }                  
              else
                NWScript.SendMessageToPC(ctx.oSender, $"Le bonus roleplay doit être compris entre 0 et 4");
            }
            else
              NWScript.SendMessageToPC(ctx.oSender, $"Cette valeur n'est pas acceptée. Le bonus roleplay doit être compris entre 0 et 4");
          }
        }
      }
      else
        NWScript.SendMessageToPC(ctx.oSender, $"Votre bonus roleplay est de { ObjectPlugin.GetInt(ctx.oSender, "_BRP")}");
    }
  }
}
