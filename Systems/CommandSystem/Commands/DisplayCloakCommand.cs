using NWN.API;

namespace NWN.Systems
{
  class DisplayCloak
  {
    public DisplayCloak(NwPlayer oPC)
    {
      NwItem oCloak = oPC.ControlledCreature.GetItemInSlot(API.Constants.InventorySlot.Cloak);

      if (oCloak != null)
      {
        if (oCloak.HiddenWhenEquipped == 0)
          oCloak.HiddenWhenEquipped = 1;
        else
          oCloak.HiddenWhenEquipped = 0;
      }
      else
        oPC.SendServerMessage("Vous ne portez pas de cape!", Color.RED);
    }
  }
}
