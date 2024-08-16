using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SacredFlame(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility); ;

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry);

          if (saveResult == SavingThrowResult.Failure || oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.EvocateurToursPuissants))
            SpellUtils.DealSpellDamage(targetCreature, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass.ClassType), saveResult);
        }
      }
    }
  }
}
