using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDisplayCloakCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NwItem oCloak = ctx.oSender.GetItemInSlot(API.Constants.InventorySlot.Cloak);

      if (oCloak != null)
      {
        if (oCloak.HiddenWhenEquipped == 0)
          oCloak.HiddenWhenEquipped = 1;
        else
          oCloak.HiddenWhenEquipped = 0;
      }
      else
        ctx.oSender.SendServerMessage("Vous ne portez pas de cape!", Color.RED);
    }
  }
}
