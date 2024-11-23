using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CompagnonAnimalRevocation(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      NwCreature companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

      if(companion is null)
      {
        caster.LoginPlayer?.SendServerMessage("Votre compagnon n'est pas invoqué", ColorConstants.Orange);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      companion.VisibilityOverride = Anvil.Services.VisibilityMode.Hidden;
      companion.Destroy();
      companion.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
      caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Delete();
      caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);
    }
  }
}
