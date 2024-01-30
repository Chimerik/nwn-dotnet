using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureProficiencyBonus(CNWSCreature creature)
    {
      byte level = creature.m_pStats.GetLevel();

      if (level < 5)
        return 2;
      else if (level < 9)
        return 3;
      else if (level < 13)
        return 4;
      else if (level < 17)
        return 5;
      else
        return 6;
    }
  }
}
