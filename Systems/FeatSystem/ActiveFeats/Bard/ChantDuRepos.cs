using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void ChantDuRepos(NwCreature caster)
    {
      if(caster.IsInCombat || caster.ActiveEffects.Any(e => e.EffectType == EffectType.Silence))
      {
        caster?.LoginPlayer.SendServerMessage("Inutilisable en combat ou sous l'effet silence", ColorConstants.Red);
        return;
      }

      foreach(var target in caster.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Reputation(ReputationType.Friend)))
      {
        if (caster.DistanceSquared(target) > 81)
          break;

        if(PlayerSystem.Players.TryGetValue(target, out var player))
          CreatureUtils.HandleShortRest(player);
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Chant du Repos")}", StringUtils.gold, true, true);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ChantDuRepos);
    }
  }
}
