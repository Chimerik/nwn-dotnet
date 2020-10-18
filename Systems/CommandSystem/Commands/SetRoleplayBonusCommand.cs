using System.Collections.Generic;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSetRoleplayBonusCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (NWScript.GetIsDM(ctx.oSender))
      {
        if (ctx.oTarget.IsValid)
        {
          if (((string)options.positional[0]).Length == 0)
          {
            NWScript.SendMessageToPC(ctx.oSender, $"Le bonus roleplay de {ctx.oTarget.Name} est de {ObjectPlugin.GetInt(ctx.oTarget, "_BRP")}");
          }
          else
          {
            int iBRP;
            if (int.TryParse((string)options.positional[0], out iBRP))
            {
              if(iBRP > -1 && iBRP < 5)
              {
                ObjectPlugin.SetInt(ctx.oTarget, "_BRP", iBRP, true);
                NWScript.SendMessageToPC(ctx.oSender, $"Le bonus roleplay de {ctx.oTarget.Name} est de { ObjectPlugin.GetInt(ctx.oTarget, "_BRP")}");
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
