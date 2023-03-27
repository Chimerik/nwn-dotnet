using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  class Daze
  {
    public Daze(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      int nCasterLevel = oCaster.LastSpellCasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);

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
          hitDice = player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? player.learnableSkills[CustomSkill.ImprovedHealth].totalPoints : 1;
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
    }
  }
}
