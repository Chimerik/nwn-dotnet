using System.Collections.Generic;
using System.Linq;
using Anvil.API;

using Ability = Anvil.API.Ability;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string KnockdownEffectTag = "_KNOCKDOWN_EFFECT";
    public static readonly Native.API.CExoString KnockdownEffectTagExo = KnockdownEffectTag.ToExoString();
    public static Effect knockdown
    {
      get
      {
        Effect eff = Effect.Knockdown();
        eff.Tag = KnockdownEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void ApplyKnockdown(NwCreature caster, NwCreature target)
    {
      CreatureSize targetSize = target.ActiveEffects.Any(e => e.Tag == EnlargeEffectTag) ? target.Size + 1 : target.Size;

      if (targetSize > CreatureSize.Large)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est de trop grande taille pour être renversée", ColorConstants.Red);
        return;
      }

      bool saveFailed = CreatureUtils.GetSkillDuelResult(caster, target, new List<Ability>() { Ability.Strength },
      new List<Ability>() { Ability.Strength, Ability.Dexterity }, new List<int>() { CustomSkill.AthleticsProficiency },
      new List<int>() { CustomSkill.AthleticsProficiency, CustomSkill.AcrobaticsProficiency }, SpellConfig.SpellEffectType.Knockdown);

      if (saveFailed)
      {
        target.ApplyEffect(EffectDuration.Temporary, knockdown,
        target.KnowsFeat((Feat)CustomSkill.Sportif) ? NwTimeSpan.FromRounds(1) : NwTimeSpan.FromRounds(2));
      }
    }
    public static void ApplyKnockdown(NwCreature creature, CreatureSize maxSize, int duration)
    {
      CreatureSize size = creature.ActiveEffects.Any(e => e.Tag == EnlargeEffectTag) ? creature.Size + 1 : creature.Size;

      if (size > maxSize)
        return;

      creature.ApplyEffect(EffectDuration.Temporary, knockdown,
        creature.KnowsFeat((Feat)CustomSkill.Sportif) ? NwTimeSpan.FromRounds(duration / 2) : NwTimeSpan.FromRounds(duration));
    }
  }
}
