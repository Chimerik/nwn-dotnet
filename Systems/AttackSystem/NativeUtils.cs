using System;
using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static class NativeUtils
  {
    public static int GetWeaponProficiencyBonus(CNWSCreature creature, CNWSItem weapon)
    {
      if (weapon is null)
        return GetProficiencyBonus(creature);

      List<Anvil.API.Feat> proficenciesRequirements = ItemUtils.GetItemProficiencies(NwBaseItem.FromItemId((int)weapon.m_nBaseItem).ItemType);

      if (proficenciesRequirements.Count < 1)
        return GetProficiencyBonus(creature);

      foreach (Anvil.API.Feat requiredProficiency in proficenciesRequirements)
        if (creature.m_pStats.HasFeat((ushort)requiredProficiency) > 0)
          return GetProficiencyBonus(creature);

      return 0;
    }
    public static int GetProficiencyBonus(CNWSCreature creature)
    {
      return creature.m_pStats.GetLevel() switch
      {
        0 or 1 or 2 or 3 or 4 => 2,
        5 or 6 or 7 or 8 => 3,
        _ => 4,
      };
    }
    public static bool IsDualWieldingLightWeapon(CNWSItem attackWeapon, int creatureSize, NwItem offHandWeapon)
    {
      if (attackWeapon is null || creatureSize < 1)
        return false;
      
      NwBaseItem baseAttackWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);

      return baseAttackWeapon.ItemType switch
      {
        BaseItemType.DireMace or  BaseItemType.Doubleaxe or BaseItemType.TwoBladedSword => true,
        _ => offHandWeapon is not null 
          && offHandWeapon.BaseItem.NumDamageDice > 0
          && offHandWeapon.BaseItem.WeaponSize > BaseItemWeaponSize.Unknown
          && (int)offHandWeapon.BaseItem.WeaponSize < creatureSize
      };
    }
    public static void SendNativeServerMessage(string message, CNWSCreature creature)
    {
      if (creature.m_bPlayerCharacter < 1)
        return;
      
      CNWCCMessageData pData = new CNWCCMessageData();
      pData.SetString(0, message.ToExoString());
      creature.SendFeedbackMessage(204, pData);
      GC.SuppressFinalize(pData);
    }
    public static int GetCritDamage(CNWSCreature attacker, CNWSItem attackWeapon, int bSneakAttack)
    {
      int damage = 0;

      if (attackWeapon is not null)
      {
        if (bSneakAttack > 0) // Hé oui, dans DD5, on reroll les dégâts des sournoises
          damage += NwRandom.Roll(Utils.random, 6, (int)Math.Ceiling((double)attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) / 2));

        NwBaseItem baseWeapon = NwBaseItem.FromItemId((int)attackWeapon.m_nBaseItem);
        damage += NwRandom.Roll(Utils.random, baseWeapon.DieToRoll, baseWeapon.NumDamageDice);
      }
      else
        damage += CreatureUtils.GetUnarmedDamage(attacker.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Monk));

      return damage;
    }
  }
}
