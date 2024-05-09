using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetDegatsBotte(CNWSCreature creature)
    {
      int bonusDamage = creature.m_ScriptVars.GetInt(FeatSystem.BotteDamageExoVariable);
      if (bonusDamage > 0)
      {
        LogUtils.LogMessage($"Dégâts Botte Secrête : +{bonusDamage}", LogUtils.LogType.Combat);
        creature.m_ScriptVars.DestroyInt(FeatSystem.BotteDamageExoVariable);
      }

      return bonusDamage;
    }
  }
}
