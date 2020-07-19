using NWN.Enums;
using NWN.NWNX;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDisplayCloakCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var oCloak = NWScript.GetItemInSlot(Enums.InventorySlot.Cloak, ctx.oSender);

      if (NWScript.GetIsObjectValid(oCloak))
      {
        if (NWScript.GetHiddenWhenEquipped(oCloak) == 0)
          NWScript.SetHiddenWhenEquipped(oCloak, 1);
        else
          NWScript.SetHiddenWhenEquipped(oCloak, 0);
      }
      else
        NWScript.FloatingTextStringOnCreature("Vous ne portez pas de cape !", ctx.oSender, false);
    }
  }
}
