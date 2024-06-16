using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void GuerisonRayonnante(NwCreature creature)
    {
      if (!CreatureUtils.HandleBonusActionUse(creature))
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Guérison Rayonnante", StringUtils.gold, true, true);
      NWScript.AssignCommand(creature, () => creature.ApplyEffect(EffectDuration.Temporary, EffectSystem.GuerisonRayonnante(creature), NwTimeSpan.FromRounds(1)));

      int heal = NativeUtils.GetCreatureProficiencyBonus(creature)
        + creature.GetClassInfo(ClassType.Paladin).Level
        + creature.GetAbilityModifier(Ability.Charisma);

      if (heal < 1)
        heal = 1;

      VfxType healVfx = heal > 20 ? VfxType.ImpHealingG : heal > 15 ? VfxType.ImpHealingL : heal > 10 ? VfxType.ImpHealingM : VfxType.ImpHealingS;

      foreach (var target in creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
      {
        if (target.HP < 1 || creature.IsReactionTypeHostile(target) || Utils.In(target.Race.RacialType, RacialType.Construct, RacialType.Undead))
          continue;

        NWScript.AssignCommand(creature, () => target.ApplyEffect(EffectDuration.Instant,
          Effect.LinkEffects(Effect.VisualEffect(healVfx), Effect.Heal(heal))));
      }

      PaladinUtils.ConsumeOathCharge(creature);
    }
  }
}
