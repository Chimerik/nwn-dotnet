﻿using Anvil.API;
using Anvil.API.Events;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnequipCheckThreatRange(OnItemUnequip onUnequip)
    {
      NwCreature oCreature = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oCreature is null || oItem is null || oCreature.GetItemInSlot(InventorySlot.RightHand) is null)
        return;

      oCreature.RemoveEffect(EffectSystem.threatAoE);
    }
  }
}
