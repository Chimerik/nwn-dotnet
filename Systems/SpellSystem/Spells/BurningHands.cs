using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BurningHands(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation, NwFeat feat = null)
    {     
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

      if (oCaster is NwCreature caster && feat is not null && feat.Id == CustomSkill.MonkFrappeDesCendres)
      {
        caster.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(caster, 2);
        casterClass = NwClass.FromClassId(CustomClass.Monk);

        if (caster.KnowsFeat((Feat)CustomSkill.MonkIncantationElementaire))
          damageDice += 1;
      }

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, oCaster.Location.Position))
      {
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 1, 
            CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }
    }
  }
}
