﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void TestJDSSAG(PlaceableEvents.OnLeftClick onUsed)
    {
      _ = onUsed.Placeable.ActionCastSpellAt(Spell.Fireball, onUsed.ClickedBy.LoginCreature);
    }
  }
}
