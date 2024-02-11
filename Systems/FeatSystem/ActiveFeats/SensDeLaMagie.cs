using System.Linq;
using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SensDeLaMagie(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
      {
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicAwarenessAura, NwTimeSpan.FromRounds(1));
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
      }
      else
        caster.LoginPlayer?.SendServerMessage("Aucune action bonus disponible", ColorConstants.Red);
    }
  }
}
