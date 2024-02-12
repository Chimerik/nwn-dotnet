using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void DispelWildMagicEffects(NwCreature creature)
    {
      creature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 0);

      foreach (var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.WildMagicCroissanceVegetaleEffectTag:
          case EffectSystem.WildMagicEspritIntangibleEffectTag:
          case EffectSystem.wildMagicRepresaillesEffectTag:
          case EffectSystem.WildMagicRayonDeLumiereEffectTag: creature.RemoveEffect(eff); break;
        }
      }
    }
  }
}
