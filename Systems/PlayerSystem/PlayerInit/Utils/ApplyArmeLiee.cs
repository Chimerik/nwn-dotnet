using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyArmeLiee()
      {
        if(oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EldritchKnightArmeLieeInvocation))
        {
          if(oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_ARME_LIEE_ID_1").HasValue)
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.EldritchKnightArmeLieeInvocation, 100);
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.EldritchKnightArmeLieeInvocation, 0);

          if (oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_ARME_LIEE_ID_2").HasValue)
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.EldritchKnightArmeLieeInvocation2, 100);
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.EldritchKnightArmeLieeInvocation2, 0);
        }
      }
    }
  }
}
