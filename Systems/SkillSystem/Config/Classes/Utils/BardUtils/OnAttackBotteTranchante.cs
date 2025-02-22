using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static async void OnAttackBotteTranchante(OnCreatureAttack onAttack)
    {
      EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.BotteSecreteEffectTag);

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackBotteTranchante;
    }
  }
}
