using NWN.API;

namespace NWN.Systems
{
  class DisplayHelm
  {
    public DisplayHelm(NwPlayer oPC)
    {
      NwItem oHelmet = oPC.ControlledCreature.GetItemInSlot(API.Constants.InventorySlot.Head);

      if (oHelmet != null)
      {
        if (oHelmet.HiddenWhenEquipped == 0)
          oHelmet.HiddenWhenEquipped = 1;
        else
          oHelmet.HiddenWhenEquipped = 0;
      }
      else
        oPC.SendServerMessage("Vous ne portez pas de casque !", Color.RED);
    }
  }
}
