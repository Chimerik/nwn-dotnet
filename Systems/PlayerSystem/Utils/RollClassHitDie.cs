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
        return learnableSkills[skillId].currentLevel < 2 ? (byte)(hitDie + conMod) : (byte)(Utils.random.Next(hitDie / 2 + 1, hitDie + 1) + conMod);
      }
    }
  }
}
