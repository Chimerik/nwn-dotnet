using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void ClearAnimalCompanion(NwCreature companion)
    {
      if(companion.Master is not null && companion.Master.IsValid)
      {
        companion.Master.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Delete();

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireFurieBestiale))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireFurieBestiale, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireSprint))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSprint, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireDisengage))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireDisengage, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireRugissementProvoquant))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluairePatteMielleuse))
        {
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluairePatteMielleuse, 0);
          companion.Master.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireRugissementProvoquantCoolDownVariable).Delete();
        }

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireChargeSanglier))
        {
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireChargeSanglier, 0);
          companion.Master.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireChargeDuSanglierCoolDownVariable).Delete();
        }

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireRageSanglier))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRageSanglier, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauAveuglement))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauAveuglement, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireCorbeauMauvaisAugure, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireLoupMorsurePlongeante))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireLoupMorsurePlongeante, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireSpiderWeb))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderWeb, 0);

        if (companion.Master.KnowsFeat((Feat)CustomSkill.BelluaireSpiderCocoon))
          companion.Master.SetFeatRemainingUses((Feat)CustomSkill.BelluaireSpiderCocoon, 0);

        if (companion.Master.IsLoginPlayerCharacter)
          companion.Master.LoginPlayer.OnClientLeave -= OnDisconnectBelluaire;
      }

      companion.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpUnsummon));
    }
  }
}
