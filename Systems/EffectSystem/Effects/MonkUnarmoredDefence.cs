using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MonkUnarmoredDefenceEffectTag = "_MONK_UNARMORED_DEFENSE_EFFECT";
    public static void ApplyMonkUnarmoredDefenseEffect(NwCreature creature)
    {
      int wisdomModifier = creature.GetAbilityModifier(Ability.Wisdom);

      if (wisdomModifier < 1)
      {
        if (creature.IsLoginPlayerCharacter)
          NwFeat.FromFeatId(CustomSkill.MonkUnarmoredDefence).Name.ClearPlayerOverride(creature.LoginPlayer);

        EffectUtils.RemoveTaggedEffect(creature, MonkUnarmoredDefenceEffectTag);

        return;
      }

      if(creature.ActiveEffects.Any(e => e.Tag == MonkUnarmoredDefenceEffectTag))
        EffectUtils.RemoveTaggedEffect(creature, MonkUnarmoredDefenceEffectTag);

      Effect acInc = Effect.ACIncrease(wisdomModifier, ACBonus.ArmourEnchantment);
      acInc.ShowIcon = false;

      Effect eff = Effect.LinkEffects(acInc, Effect.Icon(CustomEffectIcon.MoineDefenseSansArmure));
      eff.Tag = MonkUnarmoredDefenceEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      creature.ApplyEffect(EffectDuration.Permanent, eff);
      StringUtils.DelayPlayerOverrideText(creature.LoginPlayer, NwFeat.FromFeatId(CustomSkill.MonkUnarmoredDefence).Name, $"Défense sans Armure (+{wisdomModifier})");
    }
  }
}
