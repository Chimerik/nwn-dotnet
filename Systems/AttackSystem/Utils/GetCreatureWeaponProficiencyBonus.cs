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

      List<Anvil.API.Feat> proficenciesRequirements = ItemUtils.GetItemProficiencies(NwBaseItem.FromItemId((int)weapon.m_nBaseItem).ItemType);

      if (proficenciesRequirements.Count < 1)
        return GetCreatureProficiencyBonus(creature);

      foreach (Anvil.API.Feat requiredProficiency in proficenciesRequirements)
        if (creature.m_pStats.HasFeat((ushort)requiredProficiency) > 0)
          return GetCreatureProficiencyBonus(creature);

      return 0;
    }
  }
}
