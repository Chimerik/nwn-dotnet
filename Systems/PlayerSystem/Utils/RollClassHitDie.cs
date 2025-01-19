using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public byte RollClassHitDie(int playerLevel, byte classId, int conMod)
      {
        byte hitDie = NwClass.FromClassId(classId).HitDie;

        LogUtils.LogMessage($"DV de la classe : {hitDie}", LogUtils.LogType.Learnables);

        /*byte hitPointGain;

        if (playerLevel < 2)
        {
          hitPointGain = hitDie;
          oid.SendServerMessage($"Gain de points vie max {StringUtils.ToWhitecolor(hitDie)} + {StringUtils.ToWhitecolor(conMod)} (CON) = {StringUtils.ToWhitecolor(hitPointGain + conMod)}", ColorConstants.Orange);
          LogUtils.LogMessage($"Attribution des points de vie max {hitDie} + {conMod} (CON) = {hitPointGain + conMod}", LogUtils.LogType.Learnables);
        }
        else
        {*/
          byte hitPointGain = (byte)(Utils.random.Next((hitDie / 2) + 1, hitDie + 1));
          oid.SendServerMessage($"Gain de points vie aléatoires entre {StringUtils.ToWhitecolor((hitDie / 2) + 1)} et {StringUtils.ToWhitecolor(hitDie)} + {StringUtils.ToWhitecolor(conMod)} (CON) = {StringUtils.ToWhitecolor(hitPointGain + conMod)}", ColorConstants.Orange);
          LogUtils.LogMessage($"Attribution aléatoire de PV entre {(hitDie / 2) + 1} et {hitDie} + {conMod} (CON) = {hitPointGain + conMod}", LogUtils.LogType.Learnables);
        //}
          
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
