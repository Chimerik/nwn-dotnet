using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BersekerPresenceIntimidante(NwCreature caster)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.PresenceIntimidanteUsedEffectTag))
      {
        caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.PresenceIntimidanteUsed);
        caster.DecrementRemainingFeatUses((Feat)CustomSkill.BarbarianRage);
      }
      else
        caster.SetFeatRemainingUses((Feat)CustomSkill.BersekerPresenceIntimidante, caster.GetFeatRemainingUses((Feat)CustomSkill.BarbarianRage));

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(caster.Gender == Gender.Female ? VfxType.FnfHowlWarCryFemale : VfxType.FnfHowlWarCry));

      int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Strength);

      foreach(var target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
      {
        if(caster.IsReactionTypeHostile(target) 
          && CreatureUtils.GetSavingThrow(caster, target, Ability.Wisdom, spellDC, effectType:SpellConfig.SpellEffectType.Fear) == SavingThrowResult.Failure)
        {
          EffectSystem.ApplyEffroi(target, caster, NwTimeSpan.FromRounds(10), true);
        }
      }
    }
  }
}
