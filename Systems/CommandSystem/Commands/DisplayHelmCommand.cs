using NWN.API;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteDisplayHelmCommand(ChatSystem.Context ctx, Options.Result options)
    {
      NwItem oHelmet = ctx.oSender.GetItemInSlot(API.Constants.InventorySlot.Head);

      if (oHelmet != null)
      {
        if (oHelmet.HiddenWhenEquipped == 0)
          oHelmet.HiddenWhenEquipped = 1;
        else
          oHelmet.HiddenWhenEquipped = 0;
      }
      else
        ctx.oSender.SendServerMessage("Vous ne portez pas de casque !", Color.RED);
    }
  }
}
