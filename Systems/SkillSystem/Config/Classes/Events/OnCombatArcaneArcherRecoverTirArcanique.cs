using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnCombatArcaneArcherRecoverTirArcanique(OnCombatStatusChange onStatus)
    {
      if(onStatus.CombatStatus == CombatStatus.EnterCombat && onStatus.Player.LoginCreature.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirAffaiblissant)) < 1)
      {
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirAffaiblissant));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirAgrippant));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirBannissement));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirChercheur));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirExplosif));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirOmbres));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirPerforant));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirAffaiblissant));
        onStatus.Player.LoginCreature.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.ArcaneArcherTirEnvoutant));
      }
    }
  }
}
