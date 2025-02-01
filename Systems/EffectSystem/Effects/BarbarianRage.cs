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
    private static ScriptCallbackHandle onRemoveBarbarianRageCallback;
    private static ScriptCallbackHandle onIntervalBarbarianRageCallback;
    public const string BarbarianRageEffectTag = "_EFFECT_BARBARIAN_RAGE";
    //public static readonly Native.API.CExoString barbarianRageEffectExoTag = BarbarianRageEffectTag.ToExoString();
    public const string BarbarianRageAveugleEffectTag = "_EFFECT_BARBARIAN_RAGE_AVEUGLE";
    public static Effect BarbarianRage(NwCreature caster, int spellId)
    {
      int level = caster.GetClassInfo((ClassType)CustomClass.Barbarian).Level;

      Effect resBludg = Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50);
      resBludg.ShowIcon = false;
      Effect resPierc = Effect.DamageImmunityIncrease(DamageType.Piercing, 50);
      resPierc.ShowIcon = false;
      Effect resSlash = Effect.DamageImmunityIncrease(DamageType.Slashing, 50);
      resSlash.ShowIcon = false;

      Effect eff;

      switch(spellId)
      {
        case CustomSpell.RageSauvageOurs:
        case CustomSpell.PuissanceSauvageOurs:

          Effect resAcid = Effect.DamageImmunityIncrease(DamageType.Acid, 50);
          resAcid.ShowIcon = false;
          Effect resCold = Effect.DamageImmunityIncrease(DamageType.Cold, 50);
          resCold.ShowIcon = false;
          Effect resElec = Effect.DamageImmunityIncrease(DamageType.Electrical, 50);
          resElec.ShowIcon = false;
          Effect resFire = Effect.DamageImmunityIncrease(DamageType.Fire, 50);
          resFire.ShowIcon = false;
          Effect resSonic = Effect.DamageImmunityIncrease(DamageType.Sonic, 50);
          resSonic.ShowIcon = false;
          Effect resPoison = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
          resPoison.ShowIcon = false;

          eff = Effect.LinkEffects(resBludg, resPierc, resSlash, resAcid, resCold, resElec, resFire, resSonic, resPoison,
          Effect.Icon((EffectIcon)168), Effect.Icon((EffectIcon)211), Effect.Icon((EffectIcon)212), Effect.Icon((EffectIcon)213),
          Effect.Icon((EffectIcon)205), Effect.Icon((EffectIcon)206), Effect.Icon((EffectIcon)208), Effect.Icon((EffectIcon)209),
          Effect.Icon((EffectIcon)210), Effect.Icon((EffectIcon)214),
        Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
          
          break;

        default:

        eff = Effect.LinkEffects(resBludg, resPierc, resSlash,
        Effect.Icon((EffectIcon)168), Effect.Icon((EffectIcon)211), Effect.Icon((EffectIcon)212), Effect.Icon((EffectIcon)213),
        Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));

          switch(spellId)
          {
            case CustomSpell.RageSauvageAigle:

              caster.ApplyEffect(EffectDuration.Temporary, Sprint(caster), NwTimeSpan.FromRounds(1));
              caster.ApplyEffect(EffectDuration.Temporary, disengageEffect, NwTimeSpan.FromRounds(1));

              break;

            case CustomSpell.RageSauvageLoup: caster.ApplyEffect(EffectDuration.Temporary, wolfTotemAura, TimeSpan.FromMinutes(10)); break;
            case CustomSpell.PuissanceSauvageFaucon: eff = Effect.LinkEffects(eff, Vol); break;
            case CustomSpell.PuissanceSauvageTigre: NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, LionTotem, TimeSpan.FromMinutes(10))); break;
            case CustomSpell.PuissanceSauvageBelier: 
              
              caster.OnCreatureAttack -= BarbarianUtils.OnAttackBelier; 
              caster.OnCreatureAttack += BarbarianUtils.OnAttackBelier; 
              
              break;
          }            

          break;
      }

      eff.Tag = BarbarianRageEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.IntParams[5] = level > 15 ? 4 : level > 8 ? 3 : 2;
      eff.IntParams[6] = spellId;

      return eff;
    }
    private static ScriptHandleResult OnRemoveBarbarianRage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
      target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

      target.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;
      target.OnItemEquip -= ItemSystem.OnEquipBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedRageImplacable;
      target.OnDamaged -= BarbarianUtils.OnDamagedWildMagic;
      target.OnCreatureAttack -= BarbarianUtils.OnAttackBelier;

      if (target.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle))
        EffectUtils.RemoveTaggedEffect(target, BarbarianRageAveugleEffectTag);

      if (target.KnowsFeat((Feat)CustomSkill.TotemRage))
        EffectUtils.RemoveTaggedEffect(target, WolfTotemAuraEffectTag);
      
      if (target.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
        target.SetFeatRemainingUses((Feat)CustomSkill.BersekerFrenziedStrike, 0);

      if (target.KnowsFeat((Feat)CustomSkill.WildMagicSense))
      {
        EffectUtils.RemoveTaggedEffect(target, WildMagicEspritIntangibleEffectTag, WildMagicRayonDeLumiereEffectTag, WildMagicRepresaillesEffectTag,
          LumieresProtectricesAuraEffectTag, WildMagicCroissanceVegetaleAuraEffectTag);

        target.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").Delete();
        target.SetFeatRemainingUses((Feat)CustomSkill.WildMagicTeleportation, 0);

        NwItem mainWeapon = target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_1").Value;
        NwItem secondaryWeapon = target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_2").Value;

        if(mainWeapon is not null)
        {
          foreach(var ip in mainWeapon.ItemProperties)
            if(ip.Tag == "_WILDMAGIC_ARME_INFUSEE_ITEM_PROPERTY")
              mainWeapon.RemoveItemProperty(ip);

          target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_1").Delete();
        }

        if (secondaryWeapon is not null)
        {
          foreach (var ip in secondaryWeapon.ItemProperties)
            if (ip.Tag == "_WILDMAGIC_ARME_INFUSEE_ITEM_PROPERTY")
              secondaryWeapon.RemoveItemProperty(ip);

          target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_2").Delete();
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalBarbarianRage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      
      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").HasNothing
        && !target.KnowsFeat((Feat)CustomSkill.BarbarianRagePersistante))
      {
        var bonusAction = target.ActiveEffects.FirstOrDefault(e => e.Tag == BonusActionEffectTag);

        if (bonusAction is null)
          EffectUtils.RemoveTaggedEffect(target, BarbarianRageEffectTag);
        else
        {
          target.RemoveEffect(bonusAction);
          RenewBarbarianRage(target);
        }
      }
      else
        RenewBarbarianRage(target);

      return ScriptHandleResult.Handled;
    }
    private static void RenewBarbarianRage(NwCreature caster)
    {
      caster.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

      if (caster.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").HasValue)
        caster.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 1);
    }
  }
}
