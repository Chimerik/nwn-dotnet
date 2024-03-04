using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackTueurDeMage(OnCreatureAttack onAttack)
    {
      if (!onAttack.IsRangedAttack && onAttack.Target.ActiveEffects.Any(e => e.Tag == EffectSystem.ConcentrationEffectTag))
        onAttack.Target.GetObjectVariable<LocalVariableInt>("_CONCENTRATION_DISADVANTAGE").Value = 1;
    }
  }
}
