using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SouffleDuDragonDon(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwFeat feat)
    {
      if (oCaster is not NwCreature caster)
        return;

      var souffleEffect = oCaster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.SouffleDuDragonEffectTag);

      if(souffleEffect is null)
      {
        caster.RemoveFeat((Feat)CustomSkill.SouffleDuDragonAcide);
        caster.RemoveFeat((Feat)CustomSkill.SouffleDuDragonFroid);
        caster.RemoveFeat((Feat)CustomSkill.SouffleDuDragonFeu);
        caster.RemoveFeat((Feat)CustomSkill.SouffleDuDragonElec);
        caster.RemoveFeat((Feat)CustomSkill.SouffleDuDragonPoison);

        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int damageDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
      int spellDC = souffleEffect.CasterLevel;

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, oCaster.Location.Position))
      {
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, damageDice, oCaster, 2, 
            CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 6, feat.Id), NwTimeSpan.FromRounds(1));
    }
  }
}
