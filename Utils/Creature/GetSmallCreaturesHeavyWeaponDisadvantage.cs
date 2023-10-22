﻿using NWN.Native.API;
using NWN.Systems;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetSmallCreaturesHeavyWeaponDisadvantage(CNWSCreature attacker, BaseItemType weaponType)
    {

      return (attacker.m_nCreatureSize < (int)CreatureSize.Medium || attacker.m_pStats.m_nRace == (int)RacialType.Gnome
        || attacker.m_pStats.m_nRace == (int)RacialType.Halfling) && ItemUtils.IsHeavyWeapon(weaponType)
      ? -1 : 0;
    }
  }
}