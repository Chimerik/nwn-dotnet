using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceDraconiqueEffectTag = "_RESISTANCE_DRACONIQUE_EFFECT";
    public static void ApplyResistanceDraconiqueEffect(NwCreature creature)
    {
      int charismaModifier = creature.GetAbilityModifier(Ability.Charisma);

      if (charismaModifier < 1)
      {
        if (creature.IsLoginPlayerCharacter)
          NwFeat.FromFeatId(CustomSkill.EnsoResistanceDraconique).Name.ClearPlayerOverride(creature.LoginPlayer);

        EffectUtils.RemoveTaggedEffect(creature, ResistanceDraconiqueEffectTag);

        return;
      }

      if (creature.ActiveEffects.Any(e => e.Tag == ResistanceDraconiqueEffectTag))
        EffectUtils.RemoveTaggedEffect(creature, ResistanceDraconiqueEffectTag);

      Effect acInc = Effect.ACIncrease(charismaModifier, ACBonus.ArmourEnchantment);
      acInc.ShowIcon = false;

      Effect eff = Effect.LinkEffects(acInc, Effect.Icon(CustomEffectIcon.ResistanceDraconique));
      eff.Tag = ResistanceDraconiqueEffectTag;
      eff.SubType = EffectSubType.Unyielding;

      creature.ApplyEffect(EffectDuration.Permanent, eff);
      StringUtils.DelayPlayerOverrideText(creature.LoginPlayer, NwFeat.FromFeatId(CustomSkill.EnsoResistanceDraconique).Name, $"Résistance Draconique (+{charismaModifier})");
    }
  }
}
