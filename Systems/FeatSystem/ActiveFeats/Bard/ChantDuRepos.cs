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

      caster.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.DurBardSong), NwTimeSpan.FromRounds(1));

      foreach (NwCreature target in caster.Faction.GetMembers())
      {
        if (target.HP < 1 || caster.DistanceSquared(target) > 81)
          continue;

        if(PlayerSystem.Players.TryGetValue(target, out var player))
          CreatureUtils.HandleShortRest(player);
        else
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.Heal(target.MaxHP / 2));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingL));
        }
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Chant du Repos")}", StringUtils.gold, true, true);
      //caster.DecrementRemainingFeatUses((Feat)CustomSkill.ChantDuRepos);
    }
  }
}
