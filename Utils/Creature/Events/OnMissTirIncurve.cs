using Anvil.API;
using Anvil.API.Events;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnMissTirIncurve(OnCreatureAttack onAttack)
    {
      switch(onAttack.AttackResult)
      {
        case AttackResult.Miss:
        case AttackResult.MissChance:
        case AttackResult.Concealed:
        case AttackResult.Parried: TirIncurveExpiration(onAttack.Attacker); break;
      }
    }
    private static async void TirIncurveExpiration(NwCreature creature)
    {
      creature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirIncurve));
      await NwTask.Delay(NwTimeSpan.FromRounds(1));
      creature.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirIncurve));
    }
  }
}
