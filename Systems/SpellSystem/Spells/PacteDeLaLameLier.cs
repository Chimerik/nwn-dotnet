
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PacteDeLaLameLier(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (caster.IsInCombat)
      {
        caster.LoginPlayer?.SendServerMessage("Non utilisable en combat", ColorConstants.Red);
        return;
      }

      NwItem pactWeapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if (pactWeapon is null || !ItemUtils.IsMeleeWeapon(pactWeapon.BaseItem.ItemType))
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez équiper une arme de mêlée dans votre main droite afin de lier un pacte", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).HasValue)
        caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).Value?.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Delete();

      caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).Value = pactWeapon;
      pactWeapon.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.PacteDeLaLameVariable).Value = caster;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));

      caster.OnUnacquireItem -= OccultisteUtils.PacteDeLaLameOnUnacquire;
      caster.OnUnacquireItem += OccultisteUtils.PacteDeLaLameOnUnacquire;

      if (caster.KnowsFeat((Feat)CustomSkill.LameAssoiffee))
      {
        CreatureUtils.InitializeNumAttackPerRound(caster);

        caster.OnItemUnequip -= OccultisteUtils.OnUnequipPacteDeLaLame;
        caster.OnItemUnequip += OccultisteUtils.OnUnequipPacteDeLaLame;
      }
    }
  }
}
