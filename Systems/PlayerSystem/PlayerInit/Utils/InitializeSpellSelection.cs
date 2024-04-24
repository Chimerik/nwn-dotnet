using System.Collections.Generic;
using Anvil.API;

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
          if (!windows.TryGetValue("spellSelection", out var masterSpell)) windows.Add("spellSelection", new SpellSelectionWindow(this, (ClassType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CANTRIP_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RESTRICTED_SPELL_SELECTION").Value));
          else ((SpellSelectionWindow)masterSpell).CreateWindow((ClassType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CANTRIP_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_RESTRICTED_SPELL_SELECTION").Value);
        }
      }
    }
  }
}
