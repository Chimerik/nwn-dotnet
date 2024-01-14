using Anvil.API;
using Discord;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public byte RollClassHitDie(int skillId, byte classId, int conMod)
      {
        byte hitDie = NwClass.FromClassId(classId).HitDie;

        LogUtils.LogMessage($"DV de la classe : {hitDie}", LogUtils.LogType.Learnables);

        if (oid.LoginCreature.Race.Id == CustomRace.GoldDwarf)
        {
          hitDie++;
          LogUtils.LogMessage("Nain d'or : +1 point de vie", LogUtils.LogType.Learnables);
        }

        byte hitPointGain = (byte)(hitDie + conMod);

        if(learnableSkills[skillId].currentLevel > 1)
        {
          hitPointGain = (byte)(Utils.random.Next((hitDie / 2) + 1, hitDie + 1) + conMod);
          LogUtils.LogMessage($"Attribution aléatoire de PV entre {(hitDie / 2) + 1} et {hitDie} + {conMod} (CON) = {hitPointGain}", LogUtils.LogType.Learnables);
        }
        else
          LogUtils.LogMessage($"Niveau 1 : attribution des points de vie max {hitDie} + {conMod} (CON) = {hitPointGain}", LogUtils.LogType.Learnables);

        return hitPointGain;
      }
    }
  }
}
