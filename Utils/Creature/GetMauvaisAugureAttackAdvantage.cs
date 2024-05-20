using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetMauvaisAugureAttackAdvantage(Native.API.CGameEffect eff)
    {
      if(eff.m_sCustomTag.CompareNoCase(EffectSystem.mauvaisAugureEffectExoTag).ToBool())
      {
        LogUtils.LogMessage("Avantage - Mauvais Augure", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
