﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeSpellSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").HasValue)
        {
          ClassType classType = (ClassType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Value;
          int nbSpells = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").Value;
          int nbCantrips = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CANTRIP_SELECTION").Value;

          if (nbSpells > 0)
          {
            if (!windows.TryGetValue("spellSelection", out var spell)) windows.Add("spellSelection", new SpellSelectionWindow(this, classType, nbSpells));
            else ((SpellSelectionWindow)spell).CreateWindow(classType, nbSpells);
          }

          if (nbCantrips > 0)
          {
            if (!windows.TryGetValue("cantripSelection", out var cantrip)) windows.Add("cantripSelection", new CantripSelectionWindow(this, classType, nbCantrips));
            else ((CantripSelectionWindow)cantrip).CreateWindow(classType, nbCantrips);
          }

          if (nbSpells == 0 && nbCantrips == 0)
            oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Delete();
        }
      }
    }
  }
}
