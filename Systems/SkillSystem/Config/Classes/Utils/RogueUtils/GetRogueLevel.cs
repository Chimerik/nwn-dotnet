using Anvil.API;
using NWN.Native.API;
using ClassType = Anvil.API.ClassType;

namespace NWN.Systems
{
  public static partial class RogueUtils
  {
    public static byte GetRogueLevel(NwCreature creature)
    {
      if (creature.GetClassInfo(ClassType.Rogue) is not null)
        return creature.GetClassInfo(ClassType.Rogue).Level;
      else if (creature.GetClassInfo((ClassType)CustomClass.RogueArcaneTrickster) is not null)
        return creature.GetClassInfo((ClassType)CustomClass.RogueArcaneTrickster).Level;
      else return 0;
    }
    public static byte GetRogueLevel(CNWSCreature creature)
    {
      return creature.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) > creature.m_pStats.GetNumLevelsOfClass(CustomClass.RogueArcaneTrickster)
        ? (byte)creature.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Rogue) : (byte)creature.m_pStats.GetNumLevelsOfClass(CustomClass.RogueArcaneTrickster);
    }
  }
}
