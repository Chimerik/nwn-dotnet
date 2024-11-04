using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetLienDuTigreBonus(CNWSCreature attacker, CNWSObject targetObject, CNWSCombatAttackData attackData, CNWSItem attackWeapon, Anvil.API.Ability ability)
    {
      if (ability == Anvil.API.Ability.Strength
        && attacker.m_pStats.HasFeat(CustomSkill.TotemLienTigre).ToBool()
        && attackWeapon is not null
        && !attackData.m_bRangedAttack.ToBool()
        && targetObject.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.saignementEffectExoTag).ToBool()
        || (EffectTrueType)e.m_nType == EffectTrueType.Poison))
      {
        LogUtils.LogMessage("Lien du Tigre : modificateur de force x2", LogUtils.LogType.Combat);
        return 2;
      }

      return 1;
    }
  }
}
