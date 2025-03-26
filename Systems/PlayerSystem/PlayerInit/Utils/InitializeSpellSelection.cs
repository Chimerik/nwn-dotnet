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
          ClassType classType = (ClassType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Value;
          SpellSchool school = (SpellSchool)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SCHOOL_SELECTION").Value;
          int nbSpells = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SELECTION").Value;
          int nbCantrips = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CANTRIP_SELECTION").Value;
          int initiateFeatID = oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_INITIATE_CANTRIP_FEAT_ID").Value;
          string windowTag = school == SpellSchool.Unknown ? "spellSelection" : "schoolSpellSelection";

          if (nbSpells > 0)
          {
            if (!windows.TryGetValue(windowTag, out var spell)) windows.Add(windowTag, new SpellSelectionWindow(this, classType, nbSpells, school));
            else ((SpellSelectionWindow)spell).CreateWindow(classType, nbSpells, school);
          }

          if (nbCantrips > 0)
          {
            if (!windows.TryGetValue("cantripSelection", out var cantrip)) windows.Add("cantripSelection", new CantripSelectionWindow(this, classType, nbCantrips, featId: initiateFeatID));
            else ((CantripSelectionWindow)cantrip).CreateWindow(classType, nbCantrips, featId: initiateFeatID);
          }

          if (nbSpells == 0 && nbCantrips == 0)
          { 
            oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_CLASS_SELECTION").Delete();
            oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SPELL_SCHOOL_SELECTION").Delete();
          }
        }
      }
    }
  }
}
