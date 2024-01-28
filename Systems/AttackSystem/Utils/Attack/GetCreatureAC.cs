using NWN.Core;
using NWN.Core.NWNX;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureAC(CNWSCreature creature, CNWSCreature attacker)
    {
      int AC = creature.m_pStats.GetArmorClassVersus(attacker);
      uint armor = creature.m_pInventory.m_pEquipSlot[(int)InventorySlot.Chest];

      if (armor != NWScript.OBJECT_INVALID && ItemPlugin.GetBaseArmorClass(armor) > 0 && PlayerSystem.Players.TryGetValue(creature.m_idSelf, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.FighterCombatStyleDefense, out LearnableSkill defense)
        && defense.currentLevel > 0)
        AC += 1;
      
      return AC;
    }
  }
}
