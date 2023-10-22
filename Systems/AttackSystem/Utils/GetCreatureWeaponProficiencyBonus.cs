using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureWeaponProficiencyBonus(CNWSCreature creature, CNWSItem weapon)
    {
      if (weapon is null)
        return GetCreatureProficiencyBonus(creature);

      List<int> proficenciesRequirements = ItemUtils.GetItemProficiencies(NwBaseItem.FromItemId((int)weapon.m_nBaseItem).ItemType);

      if (proficenciesRequirements.Count < 1)
        return GetCreatureProficiencyBonus(creature);

      foreach (int requiredProficiency in proficenciesRequirements)
        if (creature.m_pStats.HasFeat((ushort)requiredProficiency) > 0
          || (creature.m_bPlayerCharacter > 0 && PlayerSystem.Players.TryGetValue(creature.m_idSelf, out PlayerSystem.Player player)
              && player.learnableSkills.TryGetValue(requiredProficiency, out LearnableSkill proficiency) && proficiency.currentLevel > 0) )
          return GetCreatureProficiencyBonus(creature);

      return 0;
    }
  }
}
