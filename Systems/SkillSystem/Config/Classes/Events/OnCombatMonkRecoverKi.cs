using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static void OnCombatMonkRecoverKi(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat)
      {
        NwCreature creature = onStatus.Player.LoginCreature;

        if (creature.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPatience)) < 1)
        {
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPatience), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDelugeDeCoups), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkStunStrike), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDesertion), 4);
        }
      }
    }
  }
}
