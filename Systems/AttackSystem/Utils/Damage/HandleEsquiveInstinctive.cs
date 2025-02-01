using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleEsquiveInstinctive(CNWSCreature creature)
    {
      if (creature.m_pStats.HasFeat(CustomSkill.EsquiveInstinctive).ToBool())
      {
        var reaction = creature.m_appliedEffects.FirstOrDefault(e => e.m_sCustomTag.ToString() == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          creature.RemoveEffect(reaction);
          BroadcastNativeServerMessage("Esquive Instinctive", creature);
          LogUtils.LogMessage("Esquive Instinctive : dégâts divisés par 2", LogUtils.LogType.Combat);
          return 2;
        }
      }

      return 1;
    }
  }
}
