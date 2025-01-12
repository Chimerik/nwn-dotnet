using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetVolRangedAdvantage(CNWSCreature target, bool rangedAttack)
    {
      if(rangedAttack && target is not null
        && !target.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.VolEffectTag))
      { 
        LogUtils.LogMessage("Avantage - Attaque à distance en vol contre cible à terre", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
