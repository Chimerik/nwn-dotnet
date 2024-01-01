using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;
using NWN.Core;
using NativeUtils = NWN.Systems.NativeUtils;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnAttackRenversement(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch(onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          onAttack.Attacker.OnCreatureAttack -= OnAttackRenversement;

          if (target.Size > onAttack.Attacker.Size + 1)
            onAttack.Attacker?.LoginPlayer.SendServerMessage("Impossible de renverser une créature de cette taille !", ColorConstants.Red);
          else
            EffectSystem.ApplyKnockdown(onAttack.Attacker, target);

          StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Renversement", ColorConstants.Red);

          break;
      }
    }
  }
}
