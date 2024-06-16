using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BarbarianRage(NwCreature caster)
    {
      NwItem armor = caster.GetItemInSlot(InventorySlot.Chest);

      if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage("Votre rage est déjà active", ColorConstants.Red);
        return;
      }

      if (armor is not null && armor.BaseACValue > 5)
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas entrer en rage tant que vous portez une armure lourde", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      caster.DecrementRemainingFeatUses(Feat.BarbarianRage);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} entre en {"rage".ColorString(ColorConstants.Red)}", StringUtils.gold, true, true);

      switch (NwRandom.Roll(Utils.random, 3))
      {
        case 1: caster.PlayVoiceChat(VoiceChatType.BattleCry1); break;
        case 2: caster.PlayVoiceChat(VoiceChatType.BattleCry2); break;
        case 3: caster.PlayVoiceChat(VoiceChatType.BattleCry3); break;
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

      if (caster.KnowsFeat((Feat)CustomSkill.TotemHurlementGalvanisant))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemHurlementGalvanisant, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemAspectTigre))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemAspectTigre, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemLienElan))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemLienElan, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemLienOurs))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.totemLienOursAura, NwTimeSpan.FromRounds(10));

      if (caster.KnowsFeat((Feat)CustomSkill.TotemLienLoup))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackLoupKnockdown;
        caster.OnCreatureAttack += CreatureUtils.OnAttackLoupKnockdown;
      }

      if (caster.KnowsFeat((Feat)CustomSkill.TotemEspritOurs))
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 100);
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.BearBarbarianRage, NwTimeSpan.FromRounds(10)));
      }
      else
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.BarbarianRage, NwTimeSpan.FromRounds(10)));

      byte barbarianLevel = caster.GetClassInfo(ClassType.Barbarian).Level;

      if (barbarianLevel > 10)
      {
        caster.OnDamaged -= CreatureUtils.OnDamagedRageImplacable;
        caster.OnDamaged += CreatureUtils.OnDamagedRageImplacable;
      }

      if (barbarianLevel < 15)
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
        caster.OnCreatureAttack += CreatureUtils.OnAttackBarbarianRage;

        caster.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;
        caster.OnDamaged += CreatureUtils.OnDamagedBarbarianRage;
      }

      caster.OnItemEquip -= ItemSystem.OnEquipBarbarianRage;
      caster.OnItemEquip += ItemSystem.OnEquipBarbarianRage;

      SpellUtils.DispelConcentrationEffects(caster);

      if (caster.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
        caster.IncrementRemainingFeatUses((Feat)CustomSkill.BersekerFrenziedStrike);

      if (caster.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.CharmEffectTag, EffectSystem.FrightenedEffectTag);
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, 
          EffectSystem.GetCharmImmunityEffect(EffectSystem.BarbarianRageAveugleEffectTag), NwTimeSpan.FromRounds(10)));
      }

      if (caster.KnowsFeat((Feat)CustomSkill.TotemEspritElan))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.elkTotemSpeed, NwTimeSpan.FromRounds(10));

      if (caster.KnowsFeat((Feat)CustomSkill.TotemEspritLoup))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wolfTotemAura, NwTimeSpan.FromRounds(10));

      bool freeRageRoll = BarbarianUtils.IsRatelTriggered(caster) && Utils.random.Next(0, 2).ToBool();

      if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level < 20) || freeRageRoll)
        FeatUtils.DecrementFeatUses(caster, (int)Feat.BarbarianRage);

      if (caster.KnowsFeat((Feat)CustomSkill.WildMagicSense))
      {
        HandleWildMagicRage(caster);

        if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 9))
          caster.OnDamaged += BarbarianUtils.OnDamagedWildMagic;
      }
    }
  }
}
