using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CooldownEffectTag = "_COOLDOWN_EFFECT";
    private static ScriptCallbackHandle onIntervalCooldownCallback;
    private static ScriptCallbackHandle onRemoveCooldownCallback;
    public static Effect Cooldown(NwCreature caster, int featId, int cooldown)
    {
      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveCooldownCallback, onIntervalHandle: onIntervalCooldownCallback, interval: TimeSpan.FromSeconds(1));
      eff.Tag = CooldownEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.IntParams[5] = featId;

      NwSpell spell;

      if (caster.IsLoginPlayerCharacter)
      {
        switch(featId)
        {
          case CustomSkill.BuveuseDeVie:

            spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeVieNecrotique);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeVieRadiant);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeViePsychique);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            break;

          case CustomSkill.ClercFrappeDivine:

            spell = NwSpell.FromSpellId(CustomSpell.FrappeDivineRadiant);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            spell = NwSpell.FromSpellId(CustomSpell.FrappeDivineNecrotique);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            break;

          default:

            var feat = NwFeat.FromFeatId(featId);
            feat.Name.SetPlayerOverride(caster.LoginPlayer, feat.Name.ToString() + $" - Rechargement ({cooldown} s)");

            break;
        }
      }

      return eff;
    }
    private static ScriptHandleResult OnIntervalCooldown(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        if(caster.IsLoginPlayerCharacter)
        {
          var eff = eventData.Effect;
          var feat = NwFeat.FromFeatId(eff.IntParams[5]);
          NwSpell spell;

          switch(feat.Id)
          {
            case CustomSkill.BuveuseDeVie:

              spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeVieNecrotique);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeVieRadiant);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeViePsychique);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              break;

            case CustomSkill.ClercFrappeDivine:

              spell = NwSpell.FromSpellId(CustomSpell.FrappeDivineRadiant);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              spell = NwSpell.FromSpellId(CustomSpell.FrappeDivineNecrotique);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              break;

            default: feat.Name.SetPlayerOverride(caster.LoginPlayer, feat.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)"); break;
          }            
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnRemoveCooldown(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        var eff = eventData.Effect;
        var feat = NwFeat.FromFeatId(eff.IntParams[5]);
        NwSpell spell;

        if (caster.IsLoginPlayerCharacter)
        {
          switch(feat.Id)
          {
            case CustomSkill.BuveuseDeVie:

              spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeVieNecrotique);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeVieRadiant);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              spell = NwSpell.FromSpellId(CustomSpell.BuveuseDeViePsychique);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              break;

            case CustomSkill.ClercFrappeDivine:

              spell = NwSpell.FromSpellId(CustomSpell.FrappeDivineRadiant);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              spell = NwSpell.FromSpellId(CustomSpell.FrappeDivineNecrotique);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              break;

            default: feat.Name.ClearPlayerOverride(caster.LoginPlayer); break;
          }            
        }

        HandleCooldown(caster, feat);
      }

      return ScriptHandleResult.Handled;
    }
    private static async void HandleCooldown(NwCreature caster, NwFeat feat)
    {
      await NwTask.NextFrame();

      switch (feat.Id)
      {
        case CustomSkill.BuveuseDeVie: ApplyBuveuseDeVie(caster); break;
        case CustomSkill.ClercFrappeDivine: ApplyFrappeDivine(caster); break;
        case CustomSkill.DefensesEnjoleuses: NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, DefensesEnjoleuses)); break;
      }
    }
  }
}

