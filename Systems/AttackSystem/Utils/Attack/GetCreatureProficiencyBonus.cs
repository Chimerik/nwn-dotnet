using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureProficiencyBonus(CNWSCreature creature)
    {
      if (creature.m_bPlayerCharacter.ToBool())
      { 
        byte level = creature.m_pStats.GetLevel();
        return level > 16 ? 6 : level > 12 ? 5 : level > 8 ? 4 : level > 4 ? 3 : 2;
      }
      else
      {
        float cr = creature.m_pStats.m_fChallengeRating;
        return cr > 28 ? 9 : cr > 24 ? 8 : cr > 20 ? 7 : cr > 16 ? 6 : cr > 12 ? 5 : cr > 8 ? 4 : cr > 4 ? 3 : 2; 
      }
    }
  }
}
