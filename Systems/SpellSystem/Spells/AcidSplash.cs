using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class AcidSplash
  {
    public AcidSplash(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

      Effect eVis = Effect.VisualEffect(VfxType.ImpAcidS);

      if (oCaster.CheckResistSpell(onSpellCast.TargetObject) == ResistSpellResult.Failed)
      {
        int iDamage = 3;
        int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(eVis, Effect.Damage(nDamage, DamageType.Acid)));
      }

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
      {
        oCaster.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Value = (int)onSpellCast.Spell.Id;
        oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Value = onSpellCast.TargetObject;
        oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        oCaster.OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        SpellUtils.CancelCastOnMovement(oCaster);
        //SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell.SpellType);
      }
    }
  }
}
