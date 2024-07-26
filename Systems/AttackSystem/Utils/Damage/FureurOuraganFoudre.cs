using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static async void HandleFureurOuraganFoudre(CNWSObject target, CNWSCombatAttackData data)
    {
      if (data.m_nAttackResult == 4 || data.m_bRangedAttack.ToBool() || target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) < 1
        || !target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.FureurOuraganFoudreExoTag).ToBool()))
        return;

      target.m_ScriptVars.SetInt(CreatureUtils.ReactionVariableExo, target.m_ScriptVars.GetInt(CreatureUtils.ReactionVariableExo) - 1);

      await NwTask.NextFrame();
      EffectUtils.RemoveTaggedEffect(target, EffectSystem.FureurOuraganFoudreExoTag);
    }
  }
}
