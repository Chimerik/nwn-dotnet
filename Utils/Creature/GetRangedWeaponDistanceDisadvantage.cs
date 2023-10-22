﻿using Anvil.API;
using System.Numerics;
using NWN.Native.API;
using NWN.Systems;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetRangedWeaponDistanceDisadvantage(CNWSCreature attacker, CNWSCombatAttackData attackData, BaseItemType weaponType, CNWSCreature target)
    {
      return attackData.m_bRangedAttack < 1 || target is null 
        || ItemUtils.IsRangedWeaponInOptimalRange(weaponType, Vector3.Distance(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()))
        ? 0 : -1;
    }
  }
}