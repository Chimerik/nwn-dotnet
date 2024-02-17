using Anvil.API;

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

        byte hitPointGain;

        if (learnableSkills[skillId].currentLevel > 1)
        {
          hitPointGain = (byte)(Utils.random.Next((hitDie / 2) + 1, hitDie + 1) + conMod);
          oid.SendServerMessage($"Gain de points vie aléatoires entre {StringUtils.ToWhitecolor((hitDie / 2) + 1)} et {StringUtils.ToWhitecolor(hitDie)} + {StringUtils.ToWhitecolor(conMod)} (CON) = {StringUtils.ToWhitecolor(hitPointGain)}", ColorConstants.Orange);
          LogUtils.LogMessage($"Attribution aléatoire de PV entre {(hitDie / 2) + 1} et {hitDie} + {conMod} (CON) = {hitPointGain}", LogUtils.LogType.Learnables);
        }
        else
        {
          hitPointGain = (byte)(hitDie + conMod);
          oid.SendServerMessage($"Niveau 1 : gain de points vie max {StringUtils.ToWhitecolor(hitDie)} + {StringUtils.ToWhitecolor(conMod)} (CON) = {StringUtils.ToWhitecolor(hitPointGain)}", ColorConstants.Orange);
          LogUtils.LogMessage($"Niveau 1 : attribution des points de vie max {hitDie} + {conMod} (CON) = {hitPointGain}", LogUtils.LogType.Learnables);
        }
          
        if (oid.LoginCreature.Race.Id == CustomRace.GoldDwarf)
        {
          hitPointGain++;
          oid.SendServerMessage("Nain d'or : +1 point de vie", ColorConstants.Orange);
          LogUtils.LogMessage("Nain d'or : +1 point de vie", LogUtils.LogType.Learnables);
        }

        return hitPointGain;
      }
    }
  }
}
