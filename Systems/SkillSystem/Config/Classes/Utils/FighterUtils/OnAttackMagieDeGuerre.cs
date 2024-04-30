using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnAttackMagieDeGuerre(OnCreatureAttack onAttack)
    {
      NWScript.AssignCommand(onAttack.Target, () => onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.MagieDeGuerre, NwTimeSpan.FromRounds(1)));
    }
  }
}
