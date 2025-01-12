using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFormeSauvagePanthereBonusDamage(CNWSCreature creature, CNWSCreature target)
    {
      if(target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown)
        && creature.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Polymorph 
        && e.GetInteger(0) == (int)PolymorphType.Panther))
      {
        int damage = Utils.Roll(6, 2);
        LogUtils.LogMessage($"Forme Sauvage panthère sur cible renversée : +2d6 = {damage}", LogUtils.LogType.Combat);
        return damage;
      }

      return 0;
    }
  }
}
