using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PuitsDeLuneEffectTag = "_PUITS_DE_LUNE_EFFECT";
    public static Effect PuitsDeLune(NwCreature caster, Ability ability)
    {
      Effect eff = Effect.DamageImmunityIncrease(DamageType.Divine, 50);
      eff.Tag = PuitsDeLuneEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      // + OnCreatureAttack => Si Melee + 2d6 radiant
      // + OnDamaged => Si damageur à moins de 18 m et réaction dispo, JDS CON ou aveuglement
      // + OnEffectRemoved => Virer le OnCreatureAttack, le OnDamaged et le OnEffectRemoved

      return eff;
    }
  }
}
