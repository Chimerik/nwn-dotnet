using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void WildMagicVrillesTenebreuses(NwCreature caster)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = 8 + caster.GetAbilityModifier(Ability.Constitution) + NativeUtils.GetCreatureProficiencyBonus(caster);
     
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Magie Sauvage - Vrilles Ténébreuses", StringUtils.gold, true);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosEvil20));

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
      {
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Constitution);

        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Constitution, spellDC, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, Ability.Constitution);
       
        if(saveFailed) 
          caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpNegativeEnergy),
            Effect.Damage(NwRandom.Roll(Utils.random, 12), DamageType.Negative)));
      }

      foreach (var eff in caster.ActiveEffects)
        if (eff.EffectType == EffectType.TemporaryHitpoints)
          caster.RemoveEffect(eff);

      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 12)));
    }
  }
}
