using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnAttackMorsurePlongeante(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (target.Size < CreatureSize.Large
            || !target.ActiveEffects.Any(e => e.Tag == EffectSystem.EnlargeEffectTag))
          {
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(2));
            StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Morsure Plongeante", StringUtils.gold);
          }
          //else
            //onAttack.Attacker?.Master?.LoginPlayer.SendServerMessage("Impossible de renverser une créature de cette taille !", ColorConstants.Red);

          break;
      }

      //await NwTask.NextFrame();
      //onAttack.Attacker.OnCreatureAttack -= OnAttackMorsurePlongeante;
    }
  }
}
