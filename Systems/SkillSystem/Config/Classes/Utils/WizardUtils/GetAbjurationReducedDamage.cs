using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static int GetAbjurationReducedDamage(NwGameObject target, int damage)
    {
      if (target is NwCreature creature && creature.KnowsFeat((Feat)CustomSkill.AbjurationSpellResistance))
      {
        damage /= 2;
        LogUtils.LogMessage($"Abjuration - Résistance aux sorts {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
