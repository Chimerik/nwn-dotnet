using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetAttackBonus(NwCreature target, NwCreature caster)
    {
      int attackBonus = 0;
      List<string> appliedEffects = new();

      foreach (var eff in caster.ActiveEffects)
      {
        if (eff.Tag == EffectSystem.WildMagicBienfaitEffectTag && !appliedEffects.Contains(EffectSystem.WildMagicBienfaitEffectTag))
        {
          int boonBonus = NwRandom.Roll(Utils.random, 4);
          attackBonus += boonBonus;
          appliedEffects.Add(EffectSystem.WildMagicBienfaitEffectTag);
          LogUtils.LogMessage($"Magie Sauvage : +{boonBonus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.Tag == EffectSystem.FleauEffectTag && !appliedEffects.Contains(EffectSystem.FleauEffectTag))
        {
          int fleauMalus = NwRandom.Roll(Utils.random, 4);
          attackBonus -= fleauMalus;
          appliedEffects.Add(EffectSystem.FleauEffectTag);
          LogUtils.LogMessage($"Fléau : -{fleauMalus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.Tag == EffectSystem.BenedictionEffectTag && !appliedEffects.Contains(EffectSystem.BenedictionEffectTag))
        {
          int beneBonus = NwRandom.Roll(Utils.random, 4);
          attackBonus += beneBonus;
          appliedEffects.Add(EffectSystem.FleauEffectTag);
          LogUtils.LogMessage($"Bénédiction : +{beneBonus} BA", LogUtils.LogType.Combat);
        }
        else if (eff.Tag == EffectSystem.FaveurDuMalinEffectTag && eff.IntParams[5] == CustomSpell.FaveurDuMalinAttaque)
        {
          int faveurBonus = NwRandom.Roll(Utils.random, 10);
          attackBonus += faveurBonus;
          caster.RemoveEffect(eff);
          LogUtils.LogMessage($"Faveur du malin attaque : +{faveurBonus}", LogUtils.LogType.Combat);
        }
      }

      foreach (var eff in target.ActiveEffects)
      {
        if (eff.Tag == EffectSystem.ProtectionContreLesLamesEffectTag)
        {
          int bladeWardMalus = NwRandom.Roll(Utils.random, 4);
          attackBonus -= bladeWardMalus;
          LogUtils.LogMessage($"Protection contre les lames : -{bladeWardMalus}", LogUtils.LogType.Combat);
        }
      }

      return attackBonus;
    }
  }
}
