using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetJeuDeJambeDisadvantage(CGameEffect eff)
    {
      if (eff.m_sCustomTag.ToExoLocString().GetSimple(0).ComparePrefixNoCase(EffectSystem.jeuDeJambeEffectExoTag, EffectSystem.jeuDeJambeEffectExoTag.GetLength()) > 0)
      {
        LogUtils.LogMessage("Désavantage - Cible en mode jeu de jambe", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;     
    }
  }
}
