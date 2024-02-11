using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string KnockdownEffectTag = "_KNOCKDOWN_EFFECT";
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
      bool saveFailed = CreatureUtils.GetSkillDuelResult(caster, target, new List<Ability>() { Ability.Strength },
      new List<Ability>() { Ability.Strength, Ability.Dexterity }, new List<int>() { CustomSkill.AthleticsProficiency },
      new List<int>() { CustomSkill.AthleticsProficiency, CustomSkill.AcrobaticsProficiency }, SpellConfig.SpellEffectType.Knockdown);

      if (saveFailed)
        target.ApplyEffect(EffectDuration.Temporary, knockdown, NwTimeSpan.FromRounds(1));
    }
  }
}
