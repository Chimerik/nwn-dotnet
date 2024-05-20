using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackMorsurePlongeante(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Morsure Plongeante", StringUtils.gold);

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (target.Size <= onAttack.Attacker.Size + 1)
          {
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(1));
          }
          else
            onAttack.Attacker?.Master?.LoginPlayer.SendServerMessage("Impossible de renverser une créature de cette taille !", ColorConstants.Red);

          break;
      }

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackPatteMielleuse;
    }
  }
}
