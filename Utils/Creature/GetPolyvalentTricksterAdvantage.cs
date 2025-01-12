using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetPolyvalentTricksterAdvantage(string tag, uint effCreator, CNWSCreature attacker)
    {
      if(tag == EffectSystem.ArcaneTricksterPolyvalentEffectTag
        && effCreator == attacker.m_idSelf)
      {
        LogUtils.LogMessage("Avantage - Escroc Polyvalent", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
