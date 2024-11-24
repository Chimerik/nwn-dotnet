using System.Numerics;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SpiderWeb(NwCreature caster, Vector3 oTarget)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        _ = companion.ClearActionQueue();
        _ = companion.ActionCastSpellAt(Spell.Web, Location.Create(companion.Area, oTarget, companion.Rotation), cheat:true);
        
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.BelluaireSpiderWeb));
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderWeb, 0);
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderWeb, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
