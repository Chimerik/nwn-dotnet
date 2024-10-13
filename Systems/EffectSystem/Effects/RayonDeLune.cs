using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RayonDeLuneAuraEffectTag = "_RAYON_DE_LUNE_AURA_EFFECT";
    public const string RayonDeLuneEffectTag = "_RAYON_DE_LUNE_EFFECT";
    private static ScriptCallbackHandle onEnterRayonDeLuneCallback;
    private static ScriptCallbackHandle onExitRayonDeLuneCallback;
    private static ScriptCallbackHandle onIntervalRayonDeLuneCallback;
    public static Effect RayonDeLuneAura
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.AreaOfEffect(PersistentVfxType.MobCircgood, onEnterHandle: onEnterRayonDeLuneCallback, onExitHandle: onExitRayonDeLuneCallback), 
          Effect.RunAction(onIntervalHandle: onIntervalRayonDeLuneCallback, interval:NwTimeSpan.FromRounds(1)));
        eff.Tag = RayonDeLuneAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect RayonDeLune
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.Silence);
        eff.Tag = RayonDeLuneEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterRayonDeLuneAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, RayonDeLune));
      EffectUtils.RemoveEffectType(entering, EffectType.Polymorph);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitRayonDeLuneAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, RayonDeLuneEffectTag);
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onIntervalRayonDeLuneAura(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.Effect.Creator is NwCreature caster && eventData.EffectTarget is NwCreature auraTarget)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(auraTarget, "Rayon de Lune", StringUtils.gold, true, true);
        auraTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDivineStrikeHoly));

        NwSpell spell = NwSpell.FromSpellId(CustomSpell.RayonDeLune);
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
        SpellConfig.SavingThrowFeedback feedback = new();
        int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, Ability.Wisdom);

        foreach (var target in auraTarget.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
        {
          int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, caster);

          if (target.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
          {
            advantage -= 1;
            EffectUtils.RemoveEffectType(target, EffectType.Polymorph);
          }
          
          SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spell.InnateSpellLevel, 
            CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
