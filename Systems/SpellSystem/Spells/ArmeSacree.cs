using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ArmeSacree(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NwItem target = caster.GetItemInSlot(InventorySlot.RightHand);

      if (target is null || !ItemUtils.IsMeleeWeapon(target.BaseItem))
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être équipé d'une arme de mêlée", ColorConstants.Red);
        return;
      }
      
      int charismaModifier = caster.GetAbilityModifier(Ability.Charisma) > 1 ? caster.GetAbilityModifier(Ability.Charisma) : 1;

      target.AddItemProperty(ItemProperty.AttackBonus(charismaModifier), EffectDuration.Temporary, NwTimeSpan.FromRounds(100), AddPropPolicy.KeepExisting);
      target.AddItemProperty(ItemProperty.VisualEffect(ItemVisual.Holy), EffectDuration.Temporary, NwTimeSpan.FromRounds(100), AddPropPolicy.KeepExisting);
      target.AddItemProperty(ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.White), EffectDuration.Temporary, NwTimeSpan.FromRounds(100), AddPropPolicy.KeepExisting);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
