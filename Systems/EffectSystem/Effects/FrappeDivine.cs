﻿using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeDivineEffectTag = "_FRAPPE_DIVINE_EFFECT";
    public static void ApplyFrappeDivine(NwCreature caster, NwSpell spell)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.FrappeDivine);
      eff.Tag = FrappeDivineEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Spell = spell;
      eff.Creator = caster;

      EffectUtils.RemoveTaggedEffect(caster, FrappeDivineEffectTag);

      if (!caster.ActiveEffects.Any(e => e.Tag == CooldownEffectTag && e.IntParams[5] == CustomSkill.ClercFrappeDivine))
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, eff));
      }
    }
  }
}
