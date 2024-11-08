using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetVolRangedAdvantage(CGameEffect eff, CNWSCreature attacker, CNWSCreature target, bool rangedAttack)
    {
      if(rangedAttack && eff.m_sCustomTag.CompareNoCase(EffectSystem.VolEffectExoTag).ToBool()
        && !target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.VolEffectExoTag).ToBool()))
      {
        LogUtils.LogMessage("Avantage - Attaque à distance en vol contre cible à terre", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
