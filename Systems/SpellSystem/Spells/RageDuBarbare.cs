using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RageDuBarbare(NwGameObject oCaster, NwSpell spell)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag))
      {
        caster.LoginPlayer?.SendServerMessage("Rage annulée", ColorConstants.Orange);
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.BarbarianRageEffectTag);
        return;
      }

      NwItem armor = caster.GetItemInSlot(InventorySlot.Chest);

      if (armor is not null && armor.BaseACValue > 5)
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas entrer en rage en armure lourde", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      switch (NwRandom.Roll(Utils.random, 3))
      {
        case 1: caster.PlayVoiceChat(VoiceChatType.BattleCry1); break;
        case 2: caster.PlayVoiceChat(VoiceChatType.BattleCry2); break;
        case 3: caster.PlayVoiceChat(VoiceChatType.BattleCry3); break;
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));

      if (caster.GetClassInfo(ClassType.Barbarian).Level > 6)
      {
        caster.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedIncrease(50), NwTimeSpan.FromRounds(1));
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
      }

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.BarbarianRage(caster, spell), TimeSpan.FromMinutes(10));

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
          EffectSystem.GetCharmImmunityEffect(EffectSystem.BarbarianRageAveugleEffectTag), TimeSpan.FromMinutes(10)));
      }

      if (caster.KnowsFeat((Feat)CustomSkill.WildMagicSense))
      {
        FeatSystem.HandleWildMagicRage(caster);

        if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 9))
          caster.OnDamaged += BarbarianUtils.OnDamagedWildMagic;
      }
    }
  }
}
