using Anvil.API;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static byte GetFighterLevel(NwCreature creature)
    {
      if (creature.GetClassInfo(ClassType.Fighter) is not null)
        return creature.GetClassInfo(ClassType.Fighter).Level;
      else if (creature.GetClassInfo((ClassType)CustomClass.EldritchKnight) is not null)
        return creature.GetClassInfo((ClassType)CustomClass.EldritchKnight).Level;
      else return 0;
    }
  }
}
