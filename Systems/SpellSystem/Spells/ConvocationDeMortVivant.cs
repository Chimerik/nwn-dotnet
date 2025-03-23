using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ConvocationDeMortVivant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      List<NwGameObject> concentrationList = new();

      if (oCaster is NwCreature caster)
      {
        //if (caster.Gold > 299)
        //{
          //caster.Gold -= 300;

          SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

          var summon = spell.Id switch
          {
            CustomSpell.ConvocationEspritPutride => SummonPutridSpirit(caster, casterClass),
            CustomSpell.ConvocationEspritSquelettique => SummonSquelettiqueSpirit(caster),
            _ => SummonGhostlySpirit(caster),
          };

          summon.ApplyEffect(EffectDuration.Temporary, EffectSystem.SummonDuration, SpellUtils.GetSpellDuration(oCaster, spellEntry));
          summon.ApplyEffect(EffectDuration.Permanent, EffectSystem.ImmuniteNecrotique);
          summon.ApplyEffect(EffectDuration.Permanent, EffectSystem.ImmunitePoison);
          summon.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Fear));
          summon.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Paralysis));
          summon.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Poison));
          summon.BaseAttackBonus = (byte)(NativeUtils.GetCreatureProficiencyBonus(caster) + CreatureUtils.GetAbilityModifierMin1(caster, casterClass.SpellCastingAbility));
          
          summon.Location = targetLocation;
          summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonUndead));
          
          concentrationList.Add(summon);
        //}
        //else
          //caster.LoginPlayer?.SendServerMessage("Vous devez disposer de 300 po pour faire usage de ce sort");
      }

      return concentrationList;
    }

    private static NwCreature SummonGhostlySpirit(NwCreature caster)
    {
      NwCreature summon = CreatureUtils.SummonAssociate(caster, AssociateType.Summoned, "undeadspiritghos");

      NwItem weapon = summon.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
      weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
      weapon.AddItemProperty(ItemProperty.DamageBonus(CustomItemPropertyDamageType.Necrotic, IPDamageBonus.Plus1d8), EffectDuration.Permanent);
      weapon.AddItemProperty(ItemProperty.DamageBonus(CustomItemPropertyDamageType.Necrotic, IPDamageBonus.Plus6), EffectDuration.Permanent);
      
      summon.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());
      summon.OnCreatureAttack += OnAttackGhostlySpirit;

      return summon;
    }

    private static void OnAttackGhostlySpirit(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit: EffectSystem.ApplyEffroi(target, onAttack.Attacker, NwTimeSpan.FromRounds(1), 10); break;
      }
    }

    private static NwCreature SummonPutridSpirit(NwCreature caster, NwClass casterClass)
    {
      NwCreature summon = CreatureUtils.SummonAssociate(caster, AssociateType.Summoned, "undeadspiritputr");

      NwItem weapon = summon.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
      weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
      weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
      weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus6), EffectDuration.Permanent);

      summon.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraPutride(summon, SpellUtils.GetCasterSpellDC(caster, casterClass.SpellCastingAbility)));
      summon.OnCreatureAttack += OnAttackPutridSpirit;

      return summon;
    }

    private static void OnAttackPutridSpirit(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.PoisonEffectTag))
            target.ApplyEffect(EffectDuration.Temporary, Effect.Paralyze(), NwTimeSpan.FromRounds(1));
          
          break;
      }
    }

    private static NwCreature SummonSquelettiqueSpirit(NwCreature caster)
    {
      NwCreature summon = CreatureUtils.SummonAssociate(caster, AssociateType.Summoned, "undeadspiritskel");

      NwItem weapon = summon.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
      weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
      weapon.AddItemProperty(ItemProperty.DamageBonus(CustomItemPropertyDamageType.Necrotic, IPDamageBonus.Plus6), EffectDuration.Permanent);

      return summon;
    }
  }
}
