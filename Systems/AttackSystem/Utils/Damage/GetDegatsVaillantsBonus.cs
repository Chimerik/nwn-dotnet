using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDegatsVaillantsBonus(CNWSCreature creature, CGameEffect eff)
    {
      var casterLevel = eff.m_nCasterLevel;
      creature.RemoveEffect(eff);
      LogUtils.LogMessage($"Dégâts vaillants : +{casterLevel}", LogUtils.LogType.Combat);

      return casterLevel;
    }
  }
}
