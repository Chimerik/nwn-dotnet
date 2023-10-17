using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnCombatStartForceHostility(OnCombatRoundStart onStartCombatRound)
    {
      if (onStartCombatRound.Target is NwCreature { IsPlayerControlled: true } oTarget)
        oTarget.ControllingPlayer.SetPCReputation(false, onStartCombatRound.Creature.ControllingPlayer);
    }
  }
}
