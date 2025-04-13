using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RepercussionInstableEffectTag = "_REPERCUSSION_INSTABLE_EFFECT";
    public static Effect RepercussionInstable(NwCreature caster)
    {
      caster.OnDamaged -= OnDamagedRepercussionInstable;
      caster.OnDamaged += OnDamagedRepercussionInstable;

      Effect link = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.BarbarianWildMagicRepercussionInstable), Effect.RunAction());

      link.Tag = RepercussionInstableEffectTag;
      link.SubType = EffectSubType.Unyielding;
      return link;
    }

    public static void OnDamagedRepercussionInstable(Anvil.API.Events.CreatureEvents.OnDamaged onDamaged)
    {
      var caster = onDamaged.Creature;

      if (caster.ActiveEffects.Any(e => e.Tag == BarbarianRageEffectTag)
        && CreatureUtils.HandleReactionUse(caster))
      {
        foreach(var eff in caster.ActiveEffects)
        {
          if (eff.Tag.Contains("_EFFECT_WILD_MAGIC"))
            caster.RemoveEffect(eff);
        }

        FeatSystem.HandleWildMagicRage(caster);
      }
    }
  }
}
