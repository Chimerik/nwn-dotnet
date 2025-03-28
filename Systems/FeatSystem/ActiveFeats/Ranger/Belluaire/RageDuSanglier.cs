﻿using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void RageDuSanglier(NwCreature caster)
    {
      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

      if (companion is not null)
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRageSanglier, 0);

        StringUtils.DisplayStringToAllPlayersNearTarget(companion, $"{companion.Name.ColorString(ColorConstants.Cyan)} entre en {"rage".ColorString(ColorConstants.Red)}", StringUtils.gold, false, true);

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.RageDuSanglier(caster), TimeSpan.FromMinutes(10));
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRageSanglier, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
