using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleRiposte(CNWSCreature attacker, CNWSCreature target, CNWSCombatAttackData data, string attackerName)
    {
      if (!data.m_bRangedAttack.ToBool() && target.m_ScriptVars.GetInt(CreatureUtils.ManoeuvreTypeVariableExo) == CustomSkill.WarMasterRiposte)
      {
        var reaction = target.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          target.RemoveEffect(reaction);
          target.m_ScriptVars.SetObject(CreatureUtils.ManoeuvreRiposteVariableExo, attacker.m_idSelf);

          string targetName = $"{target.GetFirstName().GetSimple(0)} {target.GetLastName().GetSimple(0)}".ColorString(ColorConstants.Cyan);
          LogUtils.LogMessage($"Riposte activation par - {targetName} contre {attackerName}", LogUtils.LogType.Combat);
        }
        else
          LogUtils.LogMessage("Riposte - Aucune réaction disponible", LogUtils.LogType.Combat);
      }
    }
  }
}
