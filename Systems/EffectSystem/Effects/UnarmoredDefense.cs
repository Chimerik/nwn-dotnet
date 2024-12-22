using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string UnarmoredDefenceEffectTag = "_UNARMORED_DEFENSE_EFFECT";
    public static void ApplyUnarmoredDefenseEffect(NwCreature creature)
    {
      int constitutionModifier = creature.GetAbilityModifier(Ability.Constitution);

      if (constitutionModifier < 1)
      {
        if (creature.IsLoginPlayerCharacter)
          NwFeat.FromFeatId(CustomSkill.BarbarianUnarmoredDefence).Name.ClearPlayerOverride(creature.LoginPlayer);

        EffectUtils.RemoveTaggedEffect(creature, UnarmoredDefenceEffectTag);

        return;
      }

      if (creature.ActiveEffects.Any(e => e.Tag == UnarmoredDefenceEffectTag))
        EffectUtils.RemoveTaggedEffect(creature, UnarmoredDefenceEffectTag);

      Effect acInc = Effect.ACIncrease(constitutionModifier, ACBonus.ArmourEnchantment);
      acInc.ShowIcon = false;

      Effect eff = Effect.LinkEffects(acInc, Effect.Icon(CustomEffectIcon.BarbareDefenseSansArmure));
      eff.Tag = UnarmoredDefenceEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      if (creature.IsLoginPlayerCharacter)
        NwFeat.FromFeatId(CustomSkill.BarbarianUnarmoredDefence).Name.SetPlayerOverride(creature.LoginPlayer, $"Défense sans Armure (+{constitutionModifier}");

      creature.ApplyEffect(EffectDuration.Permanent, eff);
    }
  }
}
