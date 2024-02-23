using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackRemoveDodge(OnCreatureAttack onAttack)
    {
      foreach (var eff in onAttack.Attacker.ActiveEffects)
        if (eff.Tag == EffectSystem.DodgeEffectTag)
          onAttack.Attacker.RemoveEffect(eff);

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackRemoveDodge;
      onAttack.Attacker.OnSpellAction -= SpellSystem.OnSpellInputRemoveDodge;
    }
  }
}
