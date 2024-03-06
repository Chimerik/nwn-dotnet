using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureProficiencyBonus(CNWSCreature creature)
    {
      byte level = creature.m_pStats.GetLevel();
      return level > 16 ? 6 : level > 12 ? 5 : level > 18 ? 4 : level > 4 ? 3 : 2;
    }
  }
}
