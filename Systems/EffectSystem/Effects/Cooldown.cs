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
    public const string CooldownEffectTag = "_COOLDOWN_EFFECT";
    private static ScriptCallbackHandle onIntervalCooldownCallback;
    private static ScriptCallbackHandle onRemoveCooldownCallback;
    public static Effect Cooldown(NwCreature caster, int cooldown, int featId, NwSpell spell = null)
    {
      Effect eff = Effect.RunAction(onRemovedHandle: onRemoveCooldownCallback, onIntervalHandle: onIntervalCooldownCallback, interval: TimeSpan.FromSeconds(1));
      eff.Tag = CooldownEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Spell = spell;
      eff.IntParams[5] = featId;
      
      NwFeat feat;

      if (caster.IsLoginPlayerCharacter)
      {
        switch (featId)
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

          case CustomSkill.ChasseurProie:

            spell = NwSpell.FromSpellId(CustomSpell.PourfendeurDeColosses);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            spell = NwSpell.FromSpellId(CustomSpell.BriseurDeHordes);
            spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({cooldown} s)");

            break;

          case CustomSkill.ProfondeursFrappeRedoutable:

            var frappeRedoutable = caster.ActiveEffects.FirstOrDefault(e => e.Tag == FrappeRedoutableEffectTag); 
            eff.CasterLevel = eff is null ? 0 : eff.CasterLevel;

            feat = NwFeat.FromFeatId(featId);
            feat.Name.SetPlayerOverride(caster.LoginPlayer, feat.Name.ToString() + $" - Rechargement ({cooldown} s)");

            break;

          case CustomSpell.Resistance:
          case CustomSpell.AuraDeVitalite:
          case BonusActionId: 
          case ReactionId: 
            break;

          default:

            feat = NwFeat.FromFeatId(featId);
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
          //ModuleSystem.Log.Info($"{feat.Name} - {eff.DurationRemaining}");
          NwSpell spell;

          switch(eff.IntParams[5])
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

            case CustomSkill.ChasseurProie:

              spell = NwSpell.FromSpellId(CustomSpell.PourfendeurDeColosses);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              spell = NwSpell.FromSpellId(CustomSpell.BriseurDeHordes);
              spell.Name.SetPlayerOverride(caster.LoginPlayer, spell.Name.ToString() + $" - Rechargement ({eff.DurationRemaining} s)");

              break;

            case CustomSpell.Resistance:
            case CustomSpell.AuraDeVitalite:
            case BonusActionId:  
            case ReactionId:  
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
        var remainingUse = eff.CasterLevel;
        NwSpell spell;

        if (caster.IsLoginPlayerCharacter)
        {
          switch(eff.IntParams[5])
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

            case CustomSkill.ChasseurProie:

              spell = NwSpell.FromSpellId(CustomSpell.PourfendeurDeColosses);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              spell = NwSpell.FromSpellId(CustomSpell.BriseurDeHordes);
              spell.Name.ClearPlayerOverride(caster.LoginPlayer);

              break;

            case CustomSpell.Resistance: 
            case BonusActionId: 
            case ReactionId: 
              break;

            default: feat.Name.ClearPlayerOverride(caster.LoginPlayer); break;
          }            
        }

        switch (eff.IntParams[5])
        {
          case BonusActionId: ApplyActionBonus(caster); break;
          case ReactionId: ApplyReaction(caster); break;
          default: HandleCooldown(caster, eff.IntParams[5], feat, eff.Spell, remainingUse); break;
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static async void HandleCooldown(NwCreature caster, int cooldownId, NwFeat feat, NwSpell spell, int remainingUse)
    {
      await NwTask.NextFrame();

      switch (cooldownId)
      {
        case CustomSkill.BuveuseDeVie: ApplyBuveuseDeVie(caster, spell); break;
        case CustomSkill.ClercFrappeDivine: ApplyFrappeDivine(caster, spell); break;
        case CustomSkill.Pourfendeur3: caster.OnCreatureAttack += RangerUtils.OnAttackPourfendeur3; break;
        case CustomSkill.DefensesEnjoleuses: NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, DefensesEnjoleuses)); break;
        case CustomSkill.MonkParade: NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, MonkParade)); break;
        
        case CustomSpell.AuraDeVitalite:
          var aura = caster.ActiveEffects.FirstOrDefault(e => e.Tag == AuraDeVitaliteEffectTag);

          if(aura is not null)
            NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, AuraDeVitaliteHeal, TimeSpan.FromSeconds(aura.DurationRemaining))); 
          
          break;
        
        case CustomSkill.ChasseurProie: 
          
          switch(spell.Id)
          {
            case CustomSpell.PourfendeurDeColosses: ApplyPourfendeurDeColosses(caster, spell); break;
            case CustomSpell.BriseurDeHordes: ApplyBriseurDeHordes(caster); break;
          }

          break;

        case CustomSkill.ProfondeursFrappeRedoutable: caster.SetFeatRemainingUses(feat.FeatType, (byte)remainingUse); break;
        case CustomSkill.BelluaireRugissementProvoquant: 
        case CustomSkill.BelluaireChargeSanglier:
        case CustomSkill.BelluaireSpiderWeb: 
        case CustomSkill.BelluaireFurieBestiale: 
        case CustomSkill.ExpertiseCommotion: 
        case CustomSkill.ExpertiseAffaiblissement: 
        case CustomSkill.ExpertiseArretCardiaque: 
        case CustomSkill.ExpertiseTranspercer: 
        case CustomSkill.ExpertiseMoulinet: 
        case CustomSkill.ExpertiseLaceration: 
        case CustomSkill.ExpertiseMutilation: 
        case CustomSkill.ExpertiseFendre: 
        case CustomSkill.ExpertiseCharge: 
        case CustomSkill.ExpertiseFrappeDuPommeau: 
        case CustomSkill.ExpertiseDesarmement: 
        case CustomSkill.ExpertiseBriseEchine: 
        case CustomSkill.ExpertiseRenforcement: 
        case CustomSkill.ExpertisePreparation: 
        case CustomSkill.ExpertiseTirPercant: 
        case CustomSkill.ExpertiseStabilisation: 
        case CustomSkill.ExpertiseCoupeJarret: 
        case CustomSkill.ExpertiseDestabiliser: 
        case CustomSkill.ExpertiseEntaille: 
          caster.SetFeatRemainingUses(feat.FeatType, 100); break;
      }
    }
  }
}

