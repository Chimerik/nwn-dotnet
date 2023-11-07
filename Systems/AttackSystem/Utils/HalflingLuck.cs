using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleHalflingLuck(CNWSCreature creature, int attackRoll)
    {
      if(attackRoll < 2 && (creature.m_pStats.m_nRace == CustomRace.LightfootHalfling || creature.m_pStats.m_nRace == CustomRace.StrongheartHalfling))
      {
        SendNativeServerMessage("Chance hafeline : jet relancé !".ColorString(StringUtils.gold), creature);
        return NwRandom.Roll(Utils.random, 20);
      }
      else
        return attackRoll;
    }
  }
}
