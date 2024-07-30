using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static async void OnAttackCrochetsDuSerpentDeFeu(OnCreatureAttack onAttack)
    {
      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          await NwTask.NextFrame();
          EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.CrochetsDuSerpentDeFeuEffectTag);
          onAttack.Attacker.ApplyEffect(EffectDuration.Temporary, EffectSystem.CrochetsDuSerpentDeFeuBis(onAttack.Attacker), NwTimeSpan.FromRounds(1));

          break;
      }
    }
  }
}
