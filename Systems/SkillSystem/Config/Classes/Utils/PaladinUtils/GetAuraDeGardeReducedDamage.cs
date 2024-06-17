using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static int GetAuraDeGardeReducedDamage(NwCreature target, int damage)
    {
      if (damage > 0 && target.ActiveEffects.Any(e => e.Tag == EffectSystem.GardeEffectTag))
      {
        damage /= 2;
        LogUtils.LogMessage($"Aura de Garde - Résistance aux sorts {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
