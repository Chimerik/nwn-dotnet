using System.Linq;
using Anvil.API;
using NWN.Native.API;
using Feat = NWN.Native.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetBarbarianRageBonusDamage(CNWSCreature creature, CNWSCombatAttackData data)
    {
      if (data.m_bRangedAttack.ToBool() || !creature.m_pStats.HasFeat((ushort)Feat.BarbarianRage).ToBool()
        || !creature.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.barbarianRageEffectExoTag).ToBool()))
        return 0;

      int barbarianLevel = creature.m_pStats.GetNumLevelsOfClass((byte)Native.API.ClassType.Barbarian);

      if (barbarianLevel < 9)
        return 2;
      else if (barbarianLevel < 16)
        return 3;
      else return 4;
    }
  }
}
