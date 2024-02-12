using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Bienfait(NwCreature caster, NwGameObject target)
    {
      if (target is not NwCreature creature)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      EffectUtils.RemoveTaggedEffect(creature, EffectSystem.WildMagicBienfaitEffectTag);

      creature.ApplyEffect(EffectDuration.Temporary, EffectSystem.wildMagicBienfait, NwTimeSpan.FromRounds(10));
      creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
    }
  }
}
