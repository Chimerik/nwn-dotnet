using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFrappeDechiranteBonus(CNWSCreature attacker, CGameEffect eff, List<string> noStack)
    {
      noStack.Add(EffectSystem.FrappeDechiranteEffectTag);

      if (attacker.m_idSelf != eff.m_oidCreator)
        return 0;

      attacker.RemoveEffect(eff);
      LogUtils.LogMessage($"Frappe Déchirante : +5 BA", LogUtils.LogType.Combat);

      return 5;
    }
  }
}
