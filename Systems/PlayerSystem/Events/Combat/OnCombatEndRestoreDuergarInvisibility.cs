using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnCombatEndRestoreDuergarInvisibility(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus != CombatStatus.ExitCombat || onStatus.Player.LoginCreature.Race.Id != CustomRace.Duergar 
        || !onStatus.Player.LoginCreature.KnowsFeat((Feat)CustomSkill.InvisibilityDuergar)
        || onStatus.Player.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.InvisibilityDuergar) > 0)
        return;

      onStatus.Player.LoginCreature.IncrementRemainingFeatUses((Feat)CustomSkill.InvisibilityDuergar);
    }
  }
}
