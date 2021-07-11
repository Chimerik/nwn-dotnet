using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  class Daze
  {
    public Daze(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell);

      Effect eMind = Effect.VisualEffect(VfxType.DurMindAffectingNegative);
      Effect eDaze = Effect.Dazed();
      Effect eDur = Effect.VisualEffect(VfxType.DurCessateNegative);

      Effect eLink = Effect.LinkEffects(eMind, eDaze);
      eLink = Effect.LinkEffects(eLink, eDur);

      Effect eVis = Effect.VisualEffect(VfxType.ImpDazedS);

      int nDuration = 2;

      //check meta magic for extend
      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration = 4;

      if (onSpellCast.TargetObject is NwCreature targetCreature)
      {
        int hitDice = 0;
        if (targetCreature.IsLoginPlayerCharacter && PlayerSystem.Players.TryGetValue(targetCreature, out PlayerSystem.Player player))
        {
          if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedHealth))
          {
            hitDice = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedHealth, player.learntCustomFeats[CustomFeats.ImprovedHealth]);
          }
          else
            hitDice = 1;
        }
        else
          hitDice = targetCreature.LevelInfo.Count;

        if (hitDice <= 5 + nCasterLevel / 6
          && oCaster.CheckResistSpell(targetCreature) == ResistSpellResult.Failed
          && targetCreature.RollSavingThrow(SavingThrow.Will, onSpellCast.SaveDC, SavingThrowType.MindSpells) == SavingThrowResult.Failure)
        {
          targetCreature.ApplyEffect(EffectDuration.Temporary, eLink, NwTimeSpan.FromRounds(nDuration));
          targetCreature.ApplyEffect(EffectDuration.Instant, eVis);
        }
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
