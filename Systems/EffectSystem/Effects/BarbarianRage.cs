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
    public const string BarbarianRageAveugleEffectTag = "_EFFECT_BARBARIAN_RAGE_AVEUGLE";
    public static Effect BarbarianRage(NwCreature caster, NwSpell spell)
    {
      int level = caster.GetClassInfo((ClassType)CustomClass.Barbarian).Level;
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.RageDuSanglier), ResistanceContondant, ResistancePercant, ResistanceTranchant,
        Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));

      switch(spell.Id)
      {
        case CustomSpell.RageSauvageOurs:
        case CustomSpell.PuissanceSauvageOurs:

          eff = Effect.LinkEffects(eff, ResistanceAcide, ResistanceFroid, ResistanceElec, ResistanceFeu, ResistancePoison, ResistanceTonnerre);
          
          break;

        case CustomSpell.RageSauvageAigle:

          caster.ApplyEffect(EffectDuration.Temporary, Sprint(caster), NwTimeSpan.FromRounds(1));
          caster.ApplyEffect(EffectDuration.Temporary, disengageEffect, NwTimeSpan.FromRounds(1));

          break;

        case CustomSpell.RageSauvageLoup: eff = Effect.LinkEffects(eff, wolfTotemAura); break;
        case CustomSpell.PuissanceSauvageFaucon: eff = Effect.LinkEffects(eff, Vol(caster)); break;
        case CustomSpell.PuissanceSauvageTigre: eff = Effect.LinkEffects(eff, LionTotem); break;
        case CustomSpell.PuissanceSauvageBelier:

          caster.OnCreatureAttack -= BarbarianUtils.OnAttackBelier;
          caster.OnCreatureAttack += BarbarianUtils.OnAttackBelier;

          break;
      }

      eff.Tag = BarbarianRageEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;

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
