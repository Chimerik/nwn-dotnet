using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BouclierEffectTag = "_BOUCLIER_EFFECT";
    private static ScriptCallbackHandle onRemoveBouclierCallback;
    public static void ApplyBouclier(NwCreature caster, NwSpell spell)
    {
      caster.OnSpellCastAt -= OnSpellCastAtBouclier;
      caster.OnSpellCastAt += OnSpellCastAtBouclier;

      Effect eff = Effect.LinkEffects(Effect.RunAction(onRemovedHandle: onRemoveSanctuaireCallback),
        Effect.SpellImmunity(Spell.MagicMissile), Effect.Icon(CustomEffectIcon.Bouclier));
      eff.Tag = BouclierEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Spell = spell;

      caster.ApplyEffect(EffectDuration.Permanent, eff);
    }
    private static ScriptHandleResult OnRemoveBouclier(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is  NwCreature creature)
      {
        creature.OnSpellCastAt -= OnSpellCastAtBouclier;

        creature.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.SpellImmunity(Spell.MagicMissile),
        Effect.ACIncrease(5), Effect.VisualEffect(VfxType.DurGlobeMinor)), NwTimeSpan.FromRounds(1));

        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));
      }

      return ScriptHandleResult.Handled;
    }
    private static void OnSpellCastAtBouclier(CreatureEvents.OnSpellCastAt onCastAt)
    {
      EffectUtils.RemoveTaggedEffect(onCastAt.Creature, BouclierEffectTag);
    }
  }
}

