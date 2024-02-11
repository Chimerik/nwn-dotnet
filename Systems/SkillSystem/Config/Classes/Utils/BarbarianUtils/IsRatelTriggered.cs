using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static bool IsRatelTriggered(NwCreature creature)
    {
      if (!creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectRatel)))
        return false;

      foreach (var eff in creature.ActiveEffects)
        switch (eff.EffectType)
        {
          case EffectType.Poison:
          case EffectType.Frightened:
          case EffectType.Charmed:
          case EffectType.Confused:
          case EffectType.Dazed:
          case EffectType.Dominated: return true;

          default:

            switch(eff.Tag)
            {
              case EffectSystem.CharmEffectTag:
              case EffectSystem.FrightenedEffectTag: return true;
            }

            break;
        }
      
      return false;
    }
  }
}
