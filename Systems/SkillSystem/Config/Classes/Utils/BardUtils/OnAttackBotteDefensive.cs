using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static async void OnAttackBotteDefensive(OnCreatureAttack onAttack)
    {
      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          var eff = onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.BotteSecreteEffectTag);
          onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetBotteDefensiveEffect(eff.CasterLevel), NwTimeSpan.FromRounds(1));

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Botte Défensive", StringUtils.gold, true);
          
          break;
      }

      EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.BotteSecreteEffectTag);

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackBotteDefensive;
    }
  }
}
