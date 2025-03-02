using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PacteDeLaLameDamageType(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      NwItem pactWeapon = caster.GetObjectVariable<LocalVariableObject<NwItem>>(CreatureUtils.PacteDeLaLameVariable).Value;

      if (pactWeapon is null)
      {
        caster.LoginPlayer?.SendServerMessage("Vous n'êtes lié à aucune arme", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      pactWeapon.GetObjectVariable<LocalVariableInt>(CreatureUtils.PacteDeLaLameVariable).Value = spell.Id;
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadOdd));

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
    }
  }
}
