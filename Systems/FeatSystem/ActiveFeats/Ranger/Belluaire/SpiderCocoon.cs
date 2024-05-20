using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SpiderCocoon(NwCreature caster, NwGameObject oTarget)
    {
      if(oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        _ = companion.ActionCastSpellAt((Spell)CustomSpell.SpiderCocoon, target, cheat:true);

        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderCocoon, 0);
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderCocoon, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
