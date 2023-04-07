using System;

using Anvil.API;
using Anvil.API.Events;

using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    private static async void HealingBreeze(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster))
        return;

      foreach (var eff in onSpellCast.TargetObject.ActiveEffects)
        if(eff.Tag.StartsWith("CUSTOM_EFFECT_REGEN_HEALING_BREEZE_"))
          onSpellCast.TargetObject.RemoveEffect(eff);

      int regen = ((int)Math.Round(4 + (double)(CreaturePlugin.GetCasterLevelOverride(oCaster, (int)SpellUtils.GetCastingClass(NwSpell.FromSpellType(Spell.Virtue)))) / 3, MidpointRounding.ToEven));
      int nDuration = 15;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      Effect healingBreeze = Effect.RunAction();
      healingBreeze = Effect.LinkEffects(healingBreeze, Effect.Icon(NwGameTables.EffectIconTable.GetRow(130)));
      healingBreeze.Tag = $"CUSTOM_EFFECT_REGEN_HEALING_BREEZE_{regen}";

      if (onSpellCast.MetaMagicFeat == MetaMagic.Extend)
        nDuration *= 2; //Duration is +100%      
      
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));

      await NwTask.NextFrame();
      onSpellCast.TargetObject.ApplyEffect(EffectDuration.Temporary, healingBreeze, TimeSpan.FromSeconds(nDuration));
    }
  }
}
