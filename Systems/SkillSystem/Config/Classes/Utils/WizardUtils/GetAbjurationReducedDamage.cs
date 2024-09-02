using Anvil.API;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static int GetAbjurationReducedDamage(NwCreature target, int damage)
    {
      if (damage > 0 && target is not null && target.KnowsFeat((Feat)CustomSkill.AbjurationSpellResistance))
      {
        damage /= 2;
        LogUtils.LogMessage($"Abjuration - Résistance aux sorts {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
