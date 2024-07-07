using System;
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

      foreach (var eff in caster.ActiveEffects)
      {
        if (eff.Tag == EffectSystem.WildMagicBienfaitEffectTag)
        {
          int boonBonus = NwRandom.Roll(Utils.random, 4);
          bonus += boonBonus;
          LogUtils.LogMessage($"Magie Sauvage : +{boonBonus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.Tag == EffectSystem.FleauEffectTag)
        {
          int fleauMalus = NwRandom.Roll(Utils.random, 4);
          bonus -= fleauMalus;
          LogUtils.LogMessage($"Fléau : -{fleauMalus} BA", LogUtils.LogType.Combat);
        }
      }

      return bonus;
    }
  }
}
