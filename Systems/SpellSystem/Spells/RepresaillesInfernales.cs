using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RepresaillesInfernales(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      if (!caster.ActiveEffects.Any(e => e.Tag == EffectSystem.DamagedByEffectTag && e.Creator == target))
      {
        caster.LoginPlayer?.SendServerMessage($"{target.Name} ne vous a pas infligé de dégât au cours du round", ColorConstants.Orange);
        return;
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      
      SavingThrowResult result = CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry);

      SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass),
         result);

      if(result == SavingThrowResult.Failure)
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));
    }
  }
}
