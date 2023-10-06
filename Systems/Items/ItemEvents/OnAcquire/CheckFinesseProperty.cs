﻿using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnAcquireCheckFinesseProperty(ModuleEvents.OnAcquireItem onAcquireItem)
    {
      switch (onAcquireItem.Item?.BaseItem.ItemType)
      {
        case BaseItemType.Shortsword:
        case BaseItemType.Dart:
        case BaseItemType.Rapier:
        case BaseItemType.Scimitar:
        case BaseItemType.Dagger:
        case BaseItemType.Whip:
          if (onAcquireItem.Item.GetObjectVariable<LocalVariableInt>("_IS_FINESSE_WEAPON").HasNothing)
            onAcquireItem.Item.GetObjectVariable<LocalVariableInt>("_IS_FINESSE_WEAPON").Value = 1;
          return;
      }
    }
  }
}
