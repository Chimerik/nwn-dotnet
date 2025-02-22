using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static Anvil.API.Ability HandleShillelagh(CNWSCreature creature, CNWSItem attackWeapon, Anvil.API.Ability attackStat)
    {
      
      if (attackWeapon is not null && Utils.In(NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem).ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff))
      {
        var shillelagh = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ShillelaghEffectTag);
        if (shillelagh is not null)
          attackStat = (Anvil.API.Ability)shillelagh.m_nCasterLevel;
      }

      return attackStat;
    }
    public static int GetShillelaghDamageDie(CNWSCreature creature, NwBaseItem weapon)
    {
      int damageDie = weapon.DieToRoll;

      if (Utils.In(weapon.ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff)
        && creature.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.ShillelaghEffectTag))
      {
        byte level = creature.m_pStats.GetLevel();
        damageDie = level > 16 ? 6 : level > 10 ? 12 : level > 4 ? 10 : 8;
      }

      return damageDie;
    }
    public static int GetShillelaghNumDice(CNWSCreature creature, NwBaseItem weapon)
    {
      int numDice = weapon.NumDamageDice;

      if (creature.m_pStats.GetLevel() > 16
        && Utils.In(weapon.ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff)
        && creature.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.ShillelaghEffectTag))
      {
        numDice = 2;
      }

      return numDice;
    }
  }
}
