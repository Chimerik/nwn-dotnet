using NWN.Core;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDisplayCloakCommand(ChatSystem.Context ctx, Options.Result options)
    {
      var oCloak = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_CLOAK, ctx.oSender);

      if (NWScript.GetIsObjectValid(oCloak) == 1)
      {
        if (NWScript.GetHiddenWhenEquipped(oCloak) == 0)
          NWScript.SetHiddenWhenEquipped(oCloak, 1);
        else
          NWScript.SetHiddenWhenEquipped(oCloak, 0);
      }
      else
        NWScript.FloatingTextStringOnCreature("Vous ne portez pas de cape !", ctx.oSender, 0);
    }
  }
}
