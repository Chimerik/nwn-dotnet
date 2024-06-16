using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string GuerisonRayonnanteEffectTag = "_GUERISON_RAYONNANTE_EFFECT";
    public static Effect GuerisonRayonnante(NwCreature caster)
    {
      Effect eff = Effect.Icon((EffectIcon)177);
      eff.Tag = GuerisonRayonnanteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      caster.OnEffectRemove += OnRemoveGuerisonRayonnante;
      return eff;
    }

    private static void OnRemoveGuerisonRayonnante(OnEffectRemove onRemove)
    {
      if (onRemove.Effect.Tag != GuerisonRayonnanteEffectTag || onRemove.Object is not NwCreature creature)
        return;

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

      creature.OnEffectRemove -= OnRemoveGuerisonRayonnante;
    }
  }
}
