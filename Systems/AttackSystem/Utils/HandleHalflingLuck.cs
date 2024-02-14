using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleHalflingLuck(CNWSCreature creature, int attackRoll)
    {
      if(attackRoll < 2 && !creature.m_ScriptVars.GetInt(EffectSystem.ChanceDebortanteCooldownExo).ToBool()
        && (creature.m_pStats.m_nRace == CustomRace.LightfootHalfling || creature.m_pStats.m_nRace == CustomRace.StrongheartHalfling))
      {
        SendNativeServerMessage("Chance hafeline".ColorString(StringUtils.gold), creature);

        int reroll = NwRandom.Roll(Utils.random, 20);
        LogUtils.LogMessage($"Chance halfeline reroll : {reroll}", LogUtils.LogType.Combat);
        
        return reroll;
      }
      else
        return attackRoll;
    }
  }
}
