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

      onRemoveHateCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveHate);

      onRemoveChargeurCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveChargeur);

      onRemoveBrandingSmiteRevealCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBrandingSmite);

      onRemoveConcentrationCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveConcentration);

      onRemoveEnlargeCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveEnlarge);

      onRemoveFaerieFireCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveFaerieFire);

      onRemoveBoneChillCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBoneChill);

      onRemoveBannissementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBannissement);

      onRemoveTirAffaiblissantCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveTirAffaiblissant);

      onApplyCharmCallback = scriptHandleFactory.CreateUniqueHandler(OnApplyCharm);
      onRemoveCharmCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveCharm);

      onIntervalTirAgrippantCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalTirAgrippant);

      onIntervalSaignementCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalSaignement);
      onRemoveSaignementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveSaignement);

      onIntervalBarbarianRageCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalBarbarianRage);
      onRemoveBarbarianRageCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBarbarianRage);

      onEnterPresenceIntimidanteStyleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterPresenceIntimidante);
      onIntervalPresenceIntimidanteCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalPresenceIntimidante);

      onEnterWolfTotemAuraCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWolfTotemAura);
      onExitWolfTotemAuraCallback = scriptHandleFactory.CreateUniqueHandler(onExitWolfTotemAura);

      onEnterElkAspectAuraCallback = scriptHandleFactory.CreateUniqueHandler(onEnterElkAspectAura);
      onExitElkAspectAuraCallback = scriptHandleFactory.CreateUniqueHandler(onExitElkAspectAura);

      onEnterWolfAspectAuraCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWolfAspectAura);
      onExitWolfAspectAuraCallback = scriptHandleFactory.CreateUniqueHandler(onExitWolfAspectAura);

      onEnterTotemLienOursCallback = scriptHandleFactory.CreateUniqueHandler(onEnterTotemLlienOursAura);
      onExitTotemLienOursCallback = scriptHandleFactory.CreateUniqueHandler(onExitTotemLienOursAura);

      onEnterTotemLienElanCallback = scriptHandleFactory.CreateUniqueHandler(onEnterTotemLienElanAura);
      onRemoveTotemLienElanCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveTotemLienElanAura);

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

      onEnterChargeDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnEnterChargeDuSanglierAura);
      onRemoveChargeDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveChargeDuSanglierAura);

      onIntervalRageDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalRageDuSanglier);
      onRemoveRageDuSanglierCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveRageDuSanglier);

      onIntervalSpiderCocoonCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalSpiderCocoon);
      onIntervalTerreurCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalTerreur);
      onIntervalChatimentDivinCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalChatimentDivin);

      onIntervalNimbeSacreeCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalNimbeSacree);
      onIntervalCourrouxDeLaNatureCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalCourrouxDeLaNature);
      onIntervalFrappePiegeuseCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalFrappePiegeuse);

      onRemoveAbjurationWardCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveAbjurationWard);

      onEnterAuraDeProtectionCallback = scriptHandleFactory.CreateUniqueHandler(onEnterProtectionAura);
      onExitAuraDeProtectionCallback = scriptHandleFactory.CreateUniqueHandler(onExitProtectionAura);

      onEnterAuraDeCourageCallback = scriptHandleFactory.CreateUniqueHandler(onEnterCourageAura);
      onExitAuraDeCourageCallback = scriptHandleFactory.CreateUniqueHandler(onExitCourageAura);

      onEnterAuraDeDevotionCallback = scriptHandleFactory.CreateUniqueHandler(onEnterDevotionAura);
      onExitAuraDeDevotionCallback = scriptHandleFactory.CreateUniqueHandler(onExitDevotionAura);

      onEnterAuraDeGardeCallback = scriptHandleFactory.CreateUniqueHandler(onEnterGardeAura);
      onExitAuraDeGardeCallback = scriptHandleFactory.CreateUniqueHandler(onExitGardeAura);

      onEnterGardienDeLaFoiCallback = scriptHandleFactory.CreateUniqueHandler(onEnterGardienDeLaFoiAura);
      onExitGardienDeLaFoiCallback = scriptHandleFactory.CreateUniqueHandler(onExitGardienDeLaFoiAura);

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

      onEnterCapeDuCroiseCallback = scriptHandleFactory.CreateUniqueHandler(onEnterCapeDuCroise);
      onExitCapeDuCroiseCallback = scriptHandleFactory.CreateUniqueHandler(onExitCapeDuCroise);

      onEnterSphereDeFeuCallback = scriptHandleFactory.CreateUniqueHandler(onEnterSphereDeFeu);
      onIntervalSphereDeFeuCallback = scriptHandleFactory.CreateUniqueHandler(onIntervalSphereDeFeu);

      onEnterTempeteDeNeigeCallback = scriptHandleFactory.CreateUniqueHandler(onEnterTempeteDeNeige);
      onHeartbeatTempeteDeNeigeCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatTempeteDeNeige);

      onEnterFleauDinsectesCallback = scriptHandleFactory.CreateUniqueHandler(onEnterFleauDinsectes);
      onHeartbeatFleauDinsectesCallback = scriptHandleFactory.CreateUniqueHandler(onHeartbeatFleauDinsectes);
      onExitFleauDinsectesCallback = scriptHandleFactory.CreateUniqueHandler(onExitFleauDinsectes);
    }
  }
}
