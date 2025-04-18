﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeFeatChoice()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FEAT_SELECTION").HasValue)
        {
          bool originOnly = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_FEAT_SELECTION").Value > 1;

          if (!windows.TryGetValue("featSelection", out var value)) windows.Add("featSelection", new FeatSelectionWindow(this, originOnly));
          else ((FeatSelectionWindow)value).CreateWindow(originOnly);
        }
      }
    }
  }
}
