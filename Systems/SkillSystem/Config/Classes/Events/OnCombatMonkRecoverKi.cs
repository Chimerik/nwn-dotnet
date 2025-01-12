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

        if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPatience) < 4)
        {
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPatience, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDelugeDeCoups, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkStunStrike, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkExplosionKi, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTenebres, 4);
          creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, 4);
        }
      }
    }
  }
}
