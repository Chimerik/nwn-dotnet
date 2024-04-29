using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnArmeLiee(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EldritchKnightArmeLiee))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EldritchKnightArmeLiee);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EldritchKnightArmeLiee2))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EldritchKnightArmeLiee2);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EldritchKnightArmeLieeInvocation))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EldritchKnightArmeLieeInvocation);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EldritchKnightArmeLieeInvocation2))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EldritchKnightArmeLieeInvocation2);

      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_1_NB_CHARGE").Value = 1;
      player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_2_NB_CHARGE").Value = 1;

      return true;
    }
  }
}
