
using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleHalflingLuck(CNWSCreature creature, int attackRoll)
    {
      return attackRoll < 2 && (creature.m_pStats.m_nRace == CustomRace.LightfootHalfling || creature.m_pStats.m_nRace == CustomRace.StrongheartHalfling)
        ? NwRandom.Roll(Utils.random, 20) : attackRoll;
    }
  }
}
