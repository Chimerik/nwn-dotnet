using Anvil.API;
using NWN.Native.API;
using Feat = Anvil.API.Feat;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleCrossbowMaster(CNWSCreature attacker, CNWSItem attackWeapon, int proficiency)
    {
      if (attackWeapon is not null && proficiency > 0 && attackWeapon.m_nBaseItem == (uint)BaseItemType.Shuriken
        && attacker.m_pStats.HasFeat((ushort)Feat.RapidReload).ToBool()
        && attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable).ToBool())
      {
        attacker.m_ScriptVars.SetInt(EffectSystem.CrossBowMasterExoTag, 1);
        attacker.m_ScriptVars.SetInt(Config.isBonusActionAvailableVariable, attacker.m_ScriptVars.GetInt(Config.isBonusActionAvailableVariable) - 1);
      }
    }
  }
}
