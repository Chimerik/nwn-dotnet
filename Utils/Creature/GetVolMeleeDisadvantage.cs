using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetVolMeleeDisadvantage(CNWSCreature attacker, CNWSCreature targe, bool rangedAttack)
    {
      if(!rangedAttack && !attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.VolEffectTag))
      {
        LogUtils.LogMessage("Désavantage - Attaque de mêlée au sol contre cible en vol", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;        
    }
  }
}
