using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeSpellSelection()
      {
        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").HasValue)
        {
          if (!windows.TryGetValue("spellSelection", out var masterSpell)) windows.Add("spellSelection", new SpellSelectionWindow(this, (ClassType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION_LEVEL").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION_NBSPELLS").Value));
          else ((SpellSelectionWindow)masterSpell).CreateWindow((ClassType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION_LEVEL").Value, oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION_NBSPELLS").Value);
        }
      }
    }
  }
}
