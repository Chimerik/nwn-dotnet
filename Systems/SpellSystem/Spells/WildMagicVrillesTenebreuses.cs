using Anvil.API;
using NWN.Core;

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
        if (target == caster)
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Constitution);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Constitution, spellDC, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, Ability.Constitution);
       
        if(saveFailed) 
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpNegativeEnergy),
            Effect.Damage(NwRandom.Roll(Utils.random, 12), CustomDamageType.Necrotic))));
      }

      EffectUtils.RemoveEffectType(caster, EffectType.TemporaryHitpoints);
      caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NwRandom.Roll(Utils.random, 12)));
    }
  }
}
