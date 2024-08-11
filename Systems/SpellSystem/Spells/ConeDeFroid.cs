using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ConeDeFroid(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation, NwFeat feat = null)
    {     
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oCaster is NwCreature castingCreature && feat is not null && feat.Id == CustomSkill.MonkSouffleDeLhiver)
      {
        castingCreature.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(castingCreature, 6);
        casterClass = NwClass.FromClassId(CustomClass.Monk);
      }

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 18, false, oCaster.Location.Position))
      {
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 5, 
          CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }
    }
  }
}
