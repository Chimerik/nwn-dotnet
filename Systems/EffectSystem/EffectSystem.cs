using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(EffectSystem))]
  public partial class EffectSystem
  {
    public static readonly Native.API.CExoString exoDelimiter = "_".ToExoString();

    private static ScriptHandleFactory scriptHandleFactory;

    public EffectSystem(ScriptHandleFactory scriptFactory)
    {
      scriptHandleFactory = scriptFactory;
      onEnterProtectionStyleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterProtectionStyle);
      onExitProtectionStyleCallback = scriptHandleFactory.CreateUniqueHandler(onExitProtectionStyle);

      onEnterContreCharmeCallback = scriptHandleFactory.CreateUniqueHandler(onEnterContreCharme);
      onExitContreCharmeCallback = scriptHandleFactory.CreateUniqueHandler(onExitContreCharme);

      onEnterChanceDebordanteCallback = scriptHandleFactory.CreateUniqueHandler(onEnterChanceDebordante);
      onExitChanceDebordanteCallback = scriptHandleFactory.CreateUniqueHandler(onExitChanceDebordante);

      onRemoveAgressionOrcCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveAgressionOrc);
      onRemoveGreleDepinesCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveGreleDepines);
      onRemoveSanctuaireCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveSanctuaire);
      onRemoveBouclierCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBouclier);
      onRemoveEntraveCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveEntrave);
      onIntervalEntraveCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalEntrave);
      onIntervalArmureDeMageCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalArmureDeMage);
      onIntervalTraitEnsorceleCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalTraitEnsorcele);

      onIntervalHeroismeCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalHeroisme);

      onRemoveViseeStableCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveViseeStable);

      onRemoveHateCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveHate);

      onRemoveChargeurCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveChargeur);

      onRemoveBrandingSmiteRevealCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBrandingSmite);

      onRemoveConcentrationCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveConcentration);

      onRemoveEnlargeCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveEnlarge);

      onRemoveFaerieFireCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveFaerieFire);

      onRemoveBoneChillCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBoneChill);

      onRemoveBannissementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBannissement);

      onRemoveTirAffaiblissantCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveTirAffaiblissant);

      onRemoveCharmCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveCharm);

      onIntervalTirAgrippantCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalTirAgrippant);

      onIntervalBrulureCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalBrulure);

      onIntervalSaignementCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalSaignement);
      onRemoveSaignementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveSaignement);

      onIntervalBarbarianRageCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalBarbarianRage);
      onRemoveBarbarianRageCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBarbarianRage);

      onEnterWolfTotemAuraCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWolfTotemAura);
      onExitWolfTotemAuraCallback = scriptHandleFactory.CreateUniqueHandler(onExitWolfTotemAura);

      onEnterLionTotemCallback = scriptHandleFactory.CreateUniqueHandler(onEnterLionTotem);
      onExitLionTotemCallback = scriptHandleFactory.CreateUniqueHandler(onExitLionTotem);

      onEnterWildMagicAwarenessCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWildMagicAwarenessAura);
      onExitWildMagicAwarenessCallback = scriptHandleFactory.CreateUniqueHandler(onExitWildMagicAwarenessAura);

      onIntervalWildMagicRayonDeLumiereCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalWildMagicRayonDeLumiere);
      onIntervalWildMagicEspritIntangibleCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalWildMagicEspritIntangible);

      onEnterWildMagicLumieresProtectricesCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWildMagicLumieresProtectricesAura);
      onExitWildMagicLumieresProtectricesCallback = scriptHandleFactory.CreateUniqueHandler(onExitWildMagicLumieresProtectricesAura);

      onRemoveWarMasterDesarmementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveWarMasterDesarmement);

      onEnterPerceptionAveugleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterPerceptionAveugle);

      onIntervalRegardHypnotiqueCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalRegardHypnotique);
      onIntervalImmobilisationDePersonneCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalImmobilisationDePersonne);
      onIntervalLenteurCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalLenteur);
      onIntervalConfusionCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalConfusion);

      onRemoveTemporaryConSaveCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveTemporaryConSave);

      onRemoveRecklessAttackCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveRecklessAttack);

      onIntervalManifestationCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalManifestation);

      onIntervalPuitsDeLuneCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalPuitsDeLune);
      onRemovePuitsDeLuneCallback = scriptHandleFactory.CreateUniqueHandler(OnRemovePuitsDeLune);

      onEnterChargeDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnEnterChargeDuSanglierAura);
      onRemoveChargeDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveChargeDuSanglierAura);

      onIntervalRageDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalRageDuSanglier);
      onRemoveRageDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveRageDuSanglier);

      onIntervalSpiderCocoonCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalSpiderCocoon);
      onIntervalTerreurCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalTerreur);

      onIntervalNimbeSacreeCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalNimbeSacree);
      onIntervalCourrouxDeLaNatureCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalCourrouxDeLaNature);
      onIntervalFrappePiegeuseCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalFrappePiegeuse);

      onRemoveAbjurationWardCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveAbjurationWard);

      onEnterAuraDeProtectionCallback = scriptHandleFactory.CreateUniqueHandler(onEnterProtectionAura);
      onExitAuraDeProtectionCallback = scriptHandleFactory.CreateUniqueHandler(onExitProtectionAura);

      onEnterGardienDeLaFoiCallback = scriptHandleFactory.CreateUniqueHandler(onEnterGardienDeLaFoiAura);
      onExitGardienDeLaFoiCallback = scriptHandleFactory.CreateUniqueHandler(onExitGardienDeLaFoiAura);

      onEnterFureurDesFlotsCallback = scriptHandleFactory.CreateUniqueHandler(onEnterFureurDesFlots);
      onHeartbeatFureurDesFlotsCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatFureurDesFlots);

      onEnterChampionAntiqueCallback = scriptHandleFactory.CreateUniqueHandler(onEnterChampionAntiqueAura);
      onExitChampionAntiqueCallback = scriptHandleFactory.CreateUniqueHandler(onExitChampionAntiqueAura);
      onHeartbeatChampionAntiqueCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatChampionAntiqueAura);
      onRemovedChampionAntiqueCallback = scriptHandleFactory.CreateUniqueHandler(onRemoveChampionAntique);

      onEnterRayonDeLuneCallback = scriptHandleFactory.CreateUniqueHandler(onEnterRayonDeLuneAura);
      onExitRayonDeLuneCallback = scriptHandleFactory.CreateUniqueHandler(onExitRayonDeLuneAura);
      onIntervalRayonDeLuneCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalRayonDeLuneAura);

      onEnterCroissanceVegetaleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterCroissanceVegetale);
      onExitCroissanceVegetaleCallback = scriptHandleFactory.CreateUniqueHandler(onExitCroissanceVegetale);

      onEnterWildMagicCroissanceVegetaleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWildMagicCroissanceVegetale);
      onExitWildMagicCroissanceVegetaleCallback = scriptHandleFactory.CreateUniqueHandler(onExitWildMagicCroissanceVegetale);

      onEnterCroissanceDepinesCallback = scriptHandleFactory.CreateUniqueHandler(onEnterCroissanceDepines);
      onExitCroissanceDepinesCallback = scriptHandleFactory.CreateUniqueHandler(onExitCroissanceDepines);
      onIntervalCroissanceDepinesCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalCroissanceDepines);

      onRemoveMarqueDuChasseurCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveMarqueDuChasseur);
      onRemoveMaleficeCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveMalefice);

      onEnterAngeDeLaVengeanceCallback = scriptHandleFactory.CreateUniqueHandler(onEnterAngeDeLaVengeance);
      onIntervalAngeDeLaVengeanceCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalAngeDeLaVengeance);

      onEnterRepliqueInvoqueeCallback = scriptHandleFactory.CreateUniqueHandler(onEnterRepliqueInvoquee);
      onExitRepliqueInvoqueeCallback = scriptHandleFactory.CreateUniqueHandler(onExitRepliqueInvoquee);
      onRemoveRepliqueInvoqueeCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveRepliqueInvoquee);

      onRemoveMaledictionDegatsCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveMaledictionDegats);
      onIntervalMaledictionEffroiCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalMaledictionEffroi);

      onEnterEspritsGardiensCallback = scriptHandleFactory.CreateUniqueHandler(onEnterEspritsGardiens);
      onExitEspritsGardiensCallback = scriptHandleFactory.CreateUniqueHandler(onExitEspritsGardiens);
      onIntervalEspritsGardiensCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalEspritsGardiens);

      onEnterVortexDechaineCallback = scriptHandleFactory.CreateUniqueHandler(onEnterVortexDechaine);
      onExitVortexDechaineCallback = scriptHandleFactory.CreateUniqueHandler(onExitVortexDechaine);
      onIntervalVortexDechaineCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalVortexDechaine);

      onEnterCapeDuCroiseCallback = scriptHandleFactory.CreateUniqueHandler(onEnterCapeDuCroise);
      onExitCapeDuCroiseCallback = scriptHandleFactory.CreateUniqueHandler(onExitCapeDuCroise);

      onEnterSphereDeFeuCallback = scriptHandleFactory.CreateUniqueHandler(onEnterSphereDeFeu);
      onIntervalSphereDeFeuCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalSphereDeFeu);

      onEnterTempeteDeNeigeCallback = scriptHandleFactory.CreateUniqueHandler(onEnterTempeteDeNeige);
      onHeartbeatTempeteDeNeigeCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatTempeteDeNeige);

      onEnterFleauDinsectesCallback = scriptHandleFactory.CreateUniqueHandler(onEnterFleauDinsectes);
      onHeartbeatFleauDinsectesCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatFleauDinsectes);
      onExitFleauDinsectesCallback = scriptHandleFactory.CreateUniqueHandler(onExitFleauDinsectes);

      onEnterBourrasqueCallback = scriptHandleFactory.CreateUniqueHandler(onEnterBourrasque);
      onHeartbeatBourrasqueCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatBourrasque);
      onExitBourrasqueCallback = scriptHandleFactory.CreateUniqueHandler(onExitBourrasque);

      onEnterNappeDeBrouillardCallback = scriptHandleFactory.CreateUniqueHandler(onEnterNappeDeBrouillard);
      onExitNappeDeBrouillardCallback = scriptHandleFactory.CreateUniqueHandler(onExitNappeDeBrouillard);

      onEnterSurfaceDeGlaceCallback = scriptHandleFactory.CreateUniqueHandler(onEnterSurfaceDeGlace);
      onHeartbeatSurfaceDeGlaceCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatSurfaceDeGlace);
      onExitSurfaceDeGlaceCallback = scriptHandleFactory.CreateUniqueHandler(onExitSurfaceDeGlace);

      onEnterEnchevetrementCallback = scriptHandleFactory.CreateUniqueHandler(onEnterEnchevetrement);
      onExitEnchevetrementCallback = scriptHandleFactory.CreateUniqueHandler(onExitEnchevetrement);

      onEnterGraisseCallback = scriptHandleFactory.CreateUniqueHandler(onEnterGraisse);
      onHeartbeatGraisseCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatGraisse);
      onExitGraisseCallback = scriptHandleFactory.CreateUniqueHandler(onExitGraisse);

      onRemoveEffroiCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveEffroi);
      onRemoveTrancheVueCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveTrancheVue);

      onIntervalExcretionCorrosiveCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalExcretionCorrosive);

      onEnterSanctuaireNaturelCallback = scriptHandleFactory.CreateUniqueHandler(onEnterSanctuaireNaturel);
      onExitSanctuaireNaturelCallback = scriptHandleFactory.CreateUniqueHandler(onExitSanctuaireNaturel);

      onIntervalPacteDeLaLameDispelCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalPacteDeLaLameDispel);

      onIntervalCooldownCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalCooldown);
      onRemoveCooldownCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveCooldown);

      onIntervalForceFantasmagoriqueCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalForceFantasmagorique);

      onRemoveDefensesEnjoleusesCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveDefensesEnjoleuses);
      onRemoveMonkParadeCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveMonkParade);

      onEnterVengeanceCalcinanteCallback = scriptHandleFactory.CreateUniqueHandler(onEnterVengeanceCalcinante);
      onExitVengeanceCalcinanteCallback = scriptHandleFactory.CreateUniqueHandler(onExitVengeanceCalcinante);

      onEnterBenedictionDuMalinCallback = scriptHandleFactory.CreateUniqueHandler(onEnterBenedictionDuMalin);
      onExitBenedictionDuMalinCallback = scriptHandleFactory.CreateUniqueHandler(onExitBenedictionDuMalin);
      onRemoveBenedictionDuMalinCallback = scriptHandleFactory.CreateUniqueHandler(onRemoveBenedictionDuMalin);

      onRemoveTraverseeInfernaleCallback = scriptHandleFactory.CreateUniqueHandler(onRemoveTraverseeInfernale);
      onRemoveMutilationCallback = scriptHandleFactory.CreateUniqueHandler(onRemoveMutilation);
      onRemoveChargeCallback = scriptHandleFactory.CreateUniqueHandler(onRemoveCharge);
      onRemoveShillelaghCallback = scriptHandleFactory.CreateUniqueHandler(onRemoveShillelagh);

      onIntervalEffroiCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalEffroi);

      onIntervalPoisonCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalPoison);

      onIntervalLacerationCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalLaceration);

      onIntervalSommeilCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalSommeil);
      onRemoveSommeilCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveSommeil);
      onIntervalFouRireDeTashaCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalFouRire);
      onRemoveFouRireDeTashaCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveFouRire);

      onIntervalDuelForceCasterCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalDuelForceCaster);
      onRemoveDuelForceCasterForceCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveDuelForceCaster);
      onRemoveDuelForceCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveDuelForce);

      onHeartbeatRayonnementInterieurCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatRayonnementInterieur);
      onHeartbeatVoileNecrotiqueCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatVoileNecrotique);

      onEnterHaloDeLumiereCallback = scriptHandleFactory.CreateUniqueHandler(onEnterHaloDeLumiere);
      onExitHaloDeLumiereCallback = scriptHandleFactory.CreateUniqueHandler(onExitHaloDeLumiere);
    }
  }
}
