using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void DispelWildMagicEffects(NwCreature creature)
    {
      creature.SetFeatRemainingUses((Feat)CustomSkill.WildMagicTeleportation, 0);

      foreach (var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.WildMagicCroissanceVegetaleAuraEffectTag:
          case EffectSystem.WildMagicEspritIntangibleEffectTag:
          case EffectSystem.WildMagicRepresaillesEffectTag:
          case EffectSystem.WildMagicRayonDeLumiereEffectTag: creature.RemoveEffect(eff); break;
        }
      }
    }
  }
}
