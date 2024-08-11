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
    public const string SphereDeFeuEffectTag = "_SPHERE_DE_FEU_EFFECT";
    public const string SphereDeFeuCooldownTag = "_SPHERE_DE_FEU_COOLDOWN_EFFECT";
    private static ScriptCallbackHandle onEnterSphereDeFeuCallback;
    private static ScriptCallbackHandle onIntervalSphereDeFeuCallback;
    public static Effect SphereDeFeu(Ability ability)
    {
      Effect eff = Effect.LinkEffects(Effect.AreaOfEffect(PersistentVfxType.MobFire, onEnterSphereDeFeuCallback, onIntervalSphereDeFeuCallback));
      eff.Tag = SphereDeFeuEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    public static Effect SphereDeFeuCooldown
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = SphereDeFeuCooldownTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterSphereDeFeu(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;
      
      if (!entering.ActiveEffects.Any(e => e.Tag == SphereDeFeuCooldownTag && e.Creator == protector))
      {
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
        entering.ApplyEffect(EffectDuration.Temporary, EspritsGardiensCooldown, TimeSpan.FromSeconds(5));

        EffectUtils.RemoveEffectType(protector, EffectType.Invisibility, EffectType.ImprovedInvisibility);
        NwSpell spell = eventData.Effect.Spell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
        int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, Ability.Wisdom);

        SpellUtils.DealSpellDamage(entering, protector.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(protector, spell), protector, 3,
          CreatureUtils.GetSavingThrow(protector, entering, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onIntervalSphereDeFeu(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Esprits Gardiens", StringUtils.gold, true, true);

      NwSpell spell = eventData.Effect.Spell;
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, Ability.Wisdom);

      foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (target == caster || !caster.IsReactionTypeHostile(target) || target.ActiveEffects.Any(e => e.Tag == EspritsGardienCooldownTag && e.Creator == caster))
          continue;

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameS));
        target.ApplyEffect(EffectDuration.Temporary, EspritsGardiensCooldown, TimeSpan.FromSeconds(5));

        SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, 3, 
          CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
