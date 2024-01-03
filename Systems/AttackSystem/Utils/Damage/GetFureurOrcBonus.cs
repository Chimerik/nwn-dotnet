using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFureurOrcBonus(CNWSCreature creature)
    {
      if (creature.m_ScriptVars.GetInt(CreatureUtils.FureurOrcBonusDamageVariableExo).ToBool()
        || creature.m_pStats.m_nRace == CustomRace.HumanoidOrc)
      {
        creature.m_ScriptVars.DestroyInt(CreatureUtils.FureurOrcBonusDamageVariableExo);
        return 1;
      }

      return 0;
    }
  }
}
