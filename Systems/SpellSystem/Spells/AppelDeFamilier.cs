using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AppelDeFamilier(NwGameObject oCaster, NwSpell spell, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.GetAssociate(AssociateType.Familiar) is not null)
      {
        caster.UnsummonFamiliar();
        return;
      }

      if (casterClass.Id != CustomClass.Druid)
      {
        if (caster.Gold < 10)
        {
          caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 10 po afin de faire usage de ce sort", ColorConstants.Red);
          return;
        }
        caster.Gold -= 10;
      }
      else
      {
        if(caster.GetFeatRemainingUses((Feat)CustomSkill.FormeSauvage) < 1)
        {
          caster.LoginPlayer?.SendServerMessage("Vous êtes à court d'utilisation de Forme Sauvage", ColorConstants.Red);
          return;
        }

        DruideUtils.DecrementFormeSauvage(caster);
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      
      NwCreature familiar = CreatureUtils.SummonAssociate(caster, AssociateType.Familiar, Familiars2da.familiarTable.FirstOrDefault(f => f.spellId == spell.Id).resRef);

      caster.OnFamiliarPossess -= OnFamiliarPossess;
      caster.OnFamiliarPossess += OnFamiliarPossess;
      caster.OnFamiliarUnpossess -= OnFamiliarUnpossess;
      caster.OnFamiliarUnpossess += OnFamiliarUnpossess;

      HandleFamiliarDamage(familiar, spell.Id);
      HandleMaitreDesChaines(caster, familiar);
    }
    private static void OnHearbeatCheckDistance(CreatureEvents.OnHeartbeat onHB)
    {
      if (onHB.Creature.Area != onHB.Creature.LoginPlayer.ControlledCreature.Area
        || onHB.Creature.DistanceSquared(onHB.Creature.LoginPlayer.ControlledCreature) > 1600)
      {
        onHB.Creature.LoginPlayer.ControlledCreature.UnpossessFamiliar();
        onHB.Creature.LoginPlayer.SendServerMessage("Votre familier est trop éloigné pour maintenir le lien télépathique", ColorConstants.Orange);
      }
    }
    private static void OnFamiliarPossess(OnFamiliarPossess onPossess)
    {
      onPossess.Owner.OnHeartbeat -= OnHearbeatCheckDistance;
      onPossess.Owner.OnHeartbeat += OnHearbeatCheckDistance;
    }
    private static void OnFamiliarUnpossess(OnFamiliarUnpossess onUnPoss)
    {
      onUnPoss.Owner.OnHeartbeat -= OnHearbeatCheckDistance;
    }
    private static void HandleMaitreDesChaines(NwCreature master, NwCreature familiar)
    {
      if(master.KnowsFeat((Feat)CustomSkill.MaitreDesChaines))
      {
        familiar.ApplyEffect(EffectDuration.Permanent, Effect.LinkEffects(EffectSystem.Vol, EffectSystem.Nage, Effect.ModifyAttacks(1), Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50),
          Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Slashing, 50)));
      }
    }
    private static void HandleFamiliarDamage(NwCreature familiar, int spellId)
    {
      NwItem weapon = familiar.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
      weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;

      switch (spellId)
      {
        case CustomSpell.AppelDeFamilierSpider:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), EffectDuration.Permanent);
          weapon.AddItemProperty(ItemProperty.DamageBonus(CustomItemPropertyDamageType.Poison, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
          familiar.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Entangle));
          break;

        case CustomSpell.AppelDeFamilierBat:
        case CustomSpell.AppelDeFamilierCrapaud:
        case CustomSpell.AppelDeFamilierRat:
        case CustomSpell.AppelDeFamilierRaven:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1), EffectDuration.Permanent);
          break;

        case CustomSpell.AppelDeFamilierCat:
        case CustomSpell.AppelDeFamilierOwl:          
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1), EffectDuration.Permanent);
          break;

        case CustomSpell.PacteDeLaChaineSerpent:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
          weapon.AddItemProperty(ItemProperty.DamageBonus(CustomItemPropertyDamageType.Poison, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
          break;

        case CustomSpell.PacteDeLaChaineDiablotin:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
          weapon.AddItemProperty(ItemProperty.DamageBonus(CustomItemPropertyDamageType.Poison, IPDamageBonus.Plus2d6), EffectDuration.Permanent);          
          familiar.ApplyEffect(EffectDuration.Permanent, Effect.LinkEffects(EffectSystem.ResistanceFroid, EffectSystem.ImmuniteFeu, EffectSystem.ImmunitePoison));
          break;

        case CustomSpell.PacteDeLaChaineEspritFollet:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
          break;

        case CustomSpell.PacteDeLaChaineSphinx:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
          break;

        case CustomSpell.PacteDeLaChainePseudoDragon:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
          familiar.ApplyEffect(EffectDuration.Permanent, Effect.ModifyAttacks(1));
          break;

        case CustomSpell.PacteDeLaChaineQuasit:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Slashing, IPDamageBonus.Plus1d4), EffectDuration.Permanent);
          familiar.OnCreatureAttack += OccultisteUtils.OnAttackQuasitPoison;
          familiar.ApplyEffect(EffectDuration.Permanent, Effect.LinkEffects(EffectSystem.ResistanceFroid, EffectSystem.ResistanceFeu, EffectSystem.ResistanceElec, EffectSystem.ImmunitePoison));
          break;

        case CustomSpell.PacteDeLaChaineSquelette:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
          familiar.ApplyEffect(EffectDuration.Permanent, Effect.Immunity(ImmunityType.Poison));
          familiar.ApplyEffect(EffectDuration.Permanent, Effect.DamageImmunityIncrease(CustomDamageType.Poison, 100));
          break;

        case CustomSpell.PacteDeLaChaineTetard:
          weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d6), EffectDuration.Permanent);
          break;
      }
    }
  }
}
