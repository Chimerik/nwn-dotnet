using Anvil.API;
using Anvil.API.Events;
using Google.Apis.Discovery;

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
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkExplosionKi), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPaumeVibratoire), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkDarkVision), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkTenebres), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkPassageSansTrace), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkSilence), 4);
          creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.MonkFrappeDombre), 4);
        }
      }
    }
  }
}
