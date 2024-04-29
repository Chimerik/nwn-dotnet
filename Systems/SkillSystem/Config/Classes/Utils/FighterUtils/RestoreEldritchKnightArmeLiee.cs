using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void RestoreEldritchKnight(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.EldritchKnightArmeLiee))
      {
       creature.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_1_NB_CHARGE").Value = 1;
       creature.GetObjectVariable<LocalVariableInt>("_ARME_LIEE_2_NB_CHARGE").Value = 1;
      }
    }
  }
}
