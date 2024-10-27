using System;
using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellAttackBonus(NwCreature caster)
    {
      int bonus = 0;

      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeGuideeVariable).HasValue)
      {
        bonus += 10;
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeGuideeVariable).Delete();
        LogUtils.LogMessage("Frappe Guidée : +10 BA", LogUtils.LogType.Combat);
      }

      List<string> appliedEffects = new();

      foreach (var eff in caster.ActiveEffects)
      {
        if (eff.Tag == EffectSystem.WildMagicBienfaitEffectTag && !appliedEffects.Contains(EffectSystem.WildMagicBienfaitEffectTag))
        {
          int boonBonus = NwRandom.Roll(Utils.random, 4);
          bonus += boonBonus;
          appliedEffects.Add(EffectSystem.WildMagicBienfaitEffectTag);
          LogUtils.LogMessage($"Magie Sauvage : +{boonBonus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.Tag == EffectSystem.FleauEffectTag && !appliedEffects.Contains(EffectSystem.FleauEffectTag))
        {
          int fleauMalus = NwRandom.Roll(Utils.random, 4);
          bonus -= fleauMalus;
          appliedEffects.Add(EffectSystem.FleauEffectTag);
          LogUtils.LogMessage($"Fléau : -{fleauMalus} BA", LogUtils.LogType.Combat);
        }
      }

      return bonus;
    }
  }
}
