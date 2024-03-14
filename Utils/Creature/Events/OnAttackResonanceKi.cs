using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackResonanceKi(OnCreatureAttack onAttack)
    {
      if (onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand) is null)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.AutomaticHit:
          case AttackResult.CriticalHit:

            if (onAttack.Target is NwCreature target)
              NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.ResonanceKi
                , NwTimeSpan.FromRounds(10)));

            StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Résonance Ki", StringUtils.gold, true);

            break;
        }
      }

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackResonanceKi;
    }
  }
}
