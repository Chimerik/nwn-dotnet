using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFrappeFrenetiqueBonus(CNWSCreature creature, CGameEffect eff, Anvil.API.Ability attackAbility, bool isCritical)
    {
      if (attackAbility == Anvil.API.Ability.Strength)
      {
        int nbDices = eff.GetInteger(5);
        int roll = Utils.Roll(6, isCritical ? nbDices * 2 : nbDices);

        creature.RemoveEffect(eff);
        LogUtils.LogMessage($"Frappe Frénétique ({nbDices}d6) : +{roll} dégâts", LogUtils.LogType.Combat);
        return roll;
      }

      return 0;  
    }
  }
}
