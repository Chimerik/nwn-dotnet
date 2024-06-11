using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ArmeSacree(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      NwItem target = null;

      if (oTarget is NwCreature targetCreature)
        target = targetCreature.GetItemInSlot(InventorySlot.RightHand);
      else if (oTarget is NwItem targetItem)
        target = targetItem;

      if (target is null || !ItemUtils.IsWeapon(target.BaseItem))
      {
        caster.LoginPlayer?.SendServerMessage("Veuillez cibler une arme", ColorConstants.Red);
        return;
      }

      if(target.Possessor != oCaster)
      {
        caster.LoginPlayer?.SendServerMessage("L'arme ciblée doit être en votre possession", ColorConstants.Red);
        return;
      }
      
      int charismaModifier = caster.GetAbilityModifier(Ability.Charisma) > 1 ? caster.GetAbilityModifier(Ability.Charisma) : 1;

      target.AddItemProperty(ItemProperty.AttackBonus(charismaModifier), EffectDuration.Temporary, NwTimeSpan.FromRounds(10), AddPropPolicy.KeepExisting);
      target.AddItemProperty(ItemProperty.VisualEffect(ItemVisual.Holy), EffectDuration.Temporary, NwTimeSpan.FromRounds(10), AddPropPolicy.KeepExisting);
      target.AddItemProperty(ItemProperty.Light(IPLightBrightness.Bright, IPLightColor.White), EffectDuration.Temporary, NwTimeSpan.FromRounds(10), AddPropPolicy.KeepExisting);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
