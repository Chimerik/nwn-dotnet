using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDisplayHelmCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var oHelmet = NWScript.GetItemInSlot(Enums.InventorySlot.Head, ctx.oSender);

      if (NWScript.GetIsObjectValid(oHelmet))
      {
        if (NWScript.GetHiddenWhenEquipped(oHelmet) == 0)
          NWScript.SetHiddenWhenEquipped(oHelmet, 1);
        else
          NWScript.SetHiddenWhenEquipped(oHelmet, 0);
      }
      else
        NWScript.FloatingTextStringOnCreature("Vous ne portez pas de casque !", ctx.oSender, false);
    }
  }
}
