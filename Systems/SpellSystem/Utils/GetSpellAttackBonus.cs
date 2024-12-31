using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSpellAttackBonus(NwCreature caster)
    {
      int bonus = 0;

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
        else if(eff.Tag == EffectSystem.FaveurDivineEffectTag && eff.IntParams[5] == CustomSpell.FaveurDuMalinAttaque)
        {
          int faveurBonus = NwRandom.Roll(Utils.random, 10);
          bonus -= faveurBonus;
          caster.RemoveEffect(eff);
          LogUtils.LogMessage($"Faveur du Malin attaque : +{faveurBonus} BA", LogUtils.LogType.Combat);
        }
      }

      return bonus;
    }
  }
}
