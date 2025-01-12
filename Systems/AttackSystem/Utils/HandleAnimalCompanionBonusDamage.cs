﻿using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAnimalCompanionBonusDamage(CNWSCreature creature, bool isCritical)
    {
      if (!isCritical && creature.m_sTag.ToString() == CreatureUtils.AnimalCompanionTag) // Si la créature est un compagnon animal, alors on ajouter le bonus de maîtrise du maître à son attaque
      {
        int bonus = GetCreatureProficiencyBonus(NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(creature.m_oidMaster));
        LogUtils.LogMessage($"Compagnon animal : +{bonus} dégâts", LogUtils.LogType.Combat);
        return bonus;
      }

      return 0;
    }
  }
}
