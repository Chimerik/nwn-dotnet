using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureProficiencyBonus(CNWSCreature creature)
    {
      return creature.m_pStats.GetLevel() switch
      {
        0 or 1 or 2 or 3 or 4 => 2,
        5 or 6 or 7 or 8 => 3,
        _ => 4,
      };
    }
  }
}
