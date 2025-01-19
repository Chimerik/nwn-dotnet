using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;
using Feat = Anvil.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureWeaponProficiencyBonus(CNWSCreature creature, CNWSItem weapon)
    {
      // Si la créature est un compagnon animal, alors on ajouter le bonus de maîtrise du maître à son attaque
      if (creature.m_sTag.CompareNoCase(CreatureUtils.AnimalCompanionTagExo).ToBool()) 
      {
        int bonus = GetCreatureProficiencyBonus(NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(creature.m_oidMaster));
        LogUtils.LogMessage($"Compagnon animal : +{bonus} BA", LogUtils.LogType.Combat);
        return bonus;
      }
      else if (creature.m_sTag.CompareNoCase(CreatureUtils.ArmeSpirituelleTagExo).ToBool())
      {
        int bonus = GetCreatureProficiencyBonus(NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(creature.m_oidMaster));
        LogUtils.LogMessage($"Arme Spirituelle : +{bonus} BA", LogUtils.LogType.Combat);
        return bonus;
      }

      if (weapon is null || !creature.m_bPlayerCharacter.ToBool() || weapon.m_ScriptVars.GetObject(CreatureUtils.PacteDeLaLameVariableExo) == creature.m_idSelf) // Si arme de pacte, alors maîtrise automatique
        return GetCreatureProficiencyBonus(creature);

      List<int> proficenciesRequirements = ItemUtils.GetItemProficiencies(NwBaseItem.FromItemId((int)weapon.m_nBaseItem).ItemType);

      if (proficenciesRequirements.Count < 1)
        return GetCreatureProficiencyBonus(creature);

      foreach (int requiredProficiency in proficenciesRequirements)
        if (creature.m_pStats.HasFeat((ushort)requiredProficiency).ToBool()
          || (creature.m_bPlayerCharacter.ToBool() && PlayerSystem.Players.TryGetValue(creature.m_idSelf, out PlayerSystem.Player player)
              && player.learnableSkills.TryGetValue(requiredProficiency, out LearnableSkill proficiency) && proficiency.currentLevel.ToBool()) )
          return GetCreatureProficiencyBonus(creature);

      return 0;
    }
    public static int GetCreatureWeaponProficiencyBonus(NwCreature creature, BaseItemType weapon)
    {
      if (weapon != BaseItemType.Invalid)
      {
        List<int> proficenciesRequirements = ItemUtils.GetItemProficiencies(weapon);

        if (proficenciesRequirements.Count < 1)
          return GetCreatureProficiencyBonus(creature);

        foreach (int requiredProficiency in proficenciesRequirements)
        {
          if (creature.KnowsFeat((Feat)requiredProficiency))
            return GetCreatureProficiencyBonus(creature);
        }
      }

      return 0;
    }
  }
}
