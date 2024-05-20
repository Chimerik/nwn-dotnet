using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFavoredEnemyDegatsBonus(CNWSCreature creature, CNWSCreature target)
    {
      if (target is null)
        return 0;

      NwFeat favoredEnemyFeat = NwRace.FromRaceId(target.m_pStats.m_nRace).GetFavoredEnemyFeat();

      if (favoredEnemyFeat is null)
        return 0;

      var master = creature.m_sTag.AsTAG().CompareNoCase(CreatureUtils.AnimalCompanionTagExo).ToBool() 
        ? NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(creature.m_oidMaster) : creature;

      if (master.m_pStats.HasFeat(favoredEnemyFeat.Id).ToBool())
      {
        if (master.m_pStats.HasFeat(CustomSkill.RangerTueurImplacable).ToBool())
        {
          int wisBonus = master.m_pStats.m_nWisdomModifier > 122 ? master.m_pStats.m_nWisdomModifier - 255 : master.m_pStats.m_nWisdomModifier;
          
          if(wisBonus < 1)
            wisBonus = 1;

          int bonusDamage = wisBonus + 4;

          LogUtils.LogMessage($"Tueur Implacable: +{bonusDamage}", LogUtils.LogType.Combat);
          return bonusDamage;
        }
        else if (master.m_pStats.HasFeat(CustomSkill.RangerGreaterFavoredEnemy).ToBool())
        {
          LogUtils.LogMessage("Grand ennemi juré : +4", LogUtils.LogType.Combat);
          return 4;
        }
        else
        {
          LogUtils.LogMessage("Ennemi juré : +2", LogUtils.LogType.Combat);
          return 2;
        }
      }

      return 0;
    }
  }
}
