using NWN.Core;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;

namespace NWN.Systems
{
  class EletricJolt
  {
    public EletricJolt(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell);

      Effect eVis = Effect.VisualEffect(VfxType.ImpLightningS);

      if (oCaster.CheckResistSpell(onSpellCast.TargetObject) == ResistSpellResult.Failed)
      {
        int iDamage = 3;
        Effect eBad = Effect.Damage(SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat), DamageType.Electrical);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eVis);
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, eBad);
      }

      if (onSpellCast.MetaMagicFeat == MetaMagic.None)
      {
        oCaster.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Value = (int)onSpellCast.Spell;
        oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Value = onSpellCast.TargetObject;
        oCaster.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
        oCaster.OnCombatRoundEnd += PlayerSystem.HandleCombatRoundEndForAutoSpells;

        SpellUtils.CancelCastOnMovement(oCaster);
        SpellUtils.RestoreSpell(oCaster, onSpellCast.Spell);
      }
    }
  }
}
