using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static void OnDamagedVengeanceCalcinante(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP < 1)
      {
        if (onDamage.Creature.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
          return;

        var aura = onDamage.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.VengeanceCalcinanteAuraEffectTag);

        if (aura is not null && aura.Creator is NwCreature protector)
        {
          int chaMod = protector.GetAbilityModifier(Ability.Charisma) > 1 ? protector.GetAbilityModifier(Ability.Charisma) : 1;
          onDamage.Creature.HP = onDamage.Creature.HP / 2;
          onDamage.Creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingX));
          onDamage.Creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfFirestorm));
          EffectUtils.RemoveTaggedEffect(protector, EffectSystem.VengeanceCalcinanteAuraEffectTag);

          foreach(var target in onDamage.Creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
          {
            if(protector.IsReactionTypeHostile(target))
            {
              NWScript.AssignCommand(protector, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 8, 2) + chaMod, DamageType.Divine)));
              NWScript.AssignCommand(protector, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Blindness(), NwTimeSpan.FromRounds(1)));
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDivineStrikeHoly));
            }
          }

          StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Creature, $"{onDamage.Creature.Name.ColorString(ColorConstants.Cyan)} - Vengeance Calcinante", StringUtils.gold, true, true);
        }

        EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.VengeanceCalcinanteEffectTag);
        onDamage.Creature.OnDamaged -= OnDamagedVengeanceCalcinante;
      }
    }
  }
}
