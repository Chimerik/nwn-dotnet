using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MainsGuerisseuses(NwCreature caster, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, 
        Effect.Heal(NativeUtils.GetCreatureProficiencyBonus(caster) * NwRandom.Roll(Utils.random, 4))));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingS));

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.MainsGuerisseuses);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Mains Guérisseuses", StringUtils.gold, true, true);
    }
  }
}
