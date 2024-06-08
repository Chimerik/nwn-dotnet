﻿using Anvil.API.Events;
using Anvil.API;
using System;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void BroadcastAttackDamage(OnCreatureDamage onDamage)
    {
      if (onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon) < 0)
        return;

      string damageTarget = onDamage.Target is NwCreature ? "blesse" : "endommage";
      int totalDamage = 0;
      

      foreach (DamageType damage in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        int damageBonus = onDamage.DamageData.GetDamageByType(damage);

        if (damageBonus < 1)
          continue;

        totalDamage += damageBonus;

        if ((int)damage < 8 || (int)damage > 32768 || damage == DamageType.BaseWeapon)
          continue;
      }

      string specialDamageString = "";

      foreach(var group in DamageTypeGroups2da.damageTypeGroupsTable)
      {
        int specialDamage = onDamage.DamageData.GetDamageByType(group.damageType);

        if (specialDamage < 1)
          continue;

        specialDamageString += $"{specialDamage} {group.tlkName.ToString()}".ColorString(group.color);
      }

      string damageString = $"{onDamage.DamagedBy.Name.ColorString(ColorConstants.Cyan)} {damageTarget} {onDamage.Target.Name.ColorString(ColorConstants.Cyan)} : " +
        $"{totalDamage}";

      if(!string.IsNullOrEmpty(damageString))
        damageString += $" ({specialDamageString})";

      foreach (var player in onDamage.Target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 35, false))
      {
        if (player == onDamage.Target || player == onDamage.DamagedBy)
          continue;

        player.LoginPlayer?.SendServerMessage(damageString, ColorConstants.Orange);
      }
    }
  }
}
