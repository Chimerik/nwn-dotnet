﻿using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ChatimentTonitruant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpHeadSonic)));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.ChatimentTonitruant(caster, spell)));
    }
  }
}
