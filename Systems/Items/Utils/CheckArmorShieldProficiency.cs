using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool CheckArmorShieldProficiency(NwCreature target, NwItem item)
    {
      if (item is null)
        return false;

      List<int> proficenciesRequirements =  GetItemProficiencies(item.BaseItem.ItemType, item.BaseACValue);

      if (proficenciesRequirements.Count < 1)
        return false;

      if (item.BaseACValue > 5) // Cas des armures lourdes
      {
        switch (target.Race.Id)
        {
          case CustomRace.Duergar:
          case CustomRace.GoldDwarf:
          case CustomRace.ShieldDwarf:
          case CustomRace.Dwarf: break;

          default:

            if (proficenciesRequirements.Contains((int)Feat.ArmorProficiencyHeavy) && target.GetAbilityScore(Ability.Strength) < 15)
              ApplyShieldArmorSlow(target);

            break;
        }
      }

      foreach (int requiredProficiency in proficenciesRequirements)
        if (target.KnowsFeat(NwFeat.FromFeatId(requiredProficiency)))
          return false;

      ApplyShieldArmorDisadvantage(target);
      target.LoginPlayer?.SendServerMessage($"Vous ne maîtrisez pas le port de {StringUtils.ToWhitecolor(item.Name)}. Malus appliqué.", ColorConstants.Red);

      return true;
    }
    private static void ApplyShieldArmorDisadvantage(NwCreature playerCreature)
    {
      playerCreature.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;
      playerCreature.OnSpellAction += SpellSystem.NoArmorShieldProficiencyOnSpellInput;

      if (!playerCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ShieldArmorDisadvantageEffectTag))
        playerCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.shieldArmorDisadvantage);
    }
    private static void ApplyShieldArmorSlow(NwCreature target)
    {
      target.OnHeartbeat -= ItemSystem.OnHeartbeatCheckHeavyArmorSlow;
      target.OnHeartbeat += ItemSystem.OnHeartbeatCheckHeavyArmorSlow;

      if (!target.ActiveEffects.Any(e => e.Tag == EffectSystem.heavyArmorSlowEffectTag))
        target.ApplyEffect(EffectDuration.Permanent, EffectSystem.heavyArmorSlow);
    }

  }
}
