﻿using Anvil.API;
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

      onEnterChanceDebordanteCallback = scriptHandleFactory.CreateUniqueHandler(onEnterChanceDebordante);
      onExitChanceDebordanteCallback = scriptHandleFactory.CreateUniqueHandler(onExitChanceDebordante);

      onRemoveAgressionOrcCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveAgressionOrc);

      onRemoveChargeurCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveChargeur);

      onRemoveBrandingSmiteRevealCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBrandingSmite);

      onRemoveConcentrationCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveConcentration);

      onRemoveEnlargeCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveEnlarge);

      onRemoveFaerieFireCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveFaerieFire);

      onRemoveBoneChillCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBoneChill);

      onRemoveTirBannissementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveTirBannissement);

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
      onIntervalWildMagicCroissanceVegetaleCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalWildMagicCroissanceVegetale);

      onEnterWildMagicLumieresProtectricesCallback = scriptHandleFactory.CreateUniqueHandler(onEnterWildMagicLumieresProtectricesAura);
      onExitWildMagicLumieresProtectricesCallback = scriptHandleFactory.CreateUniqueHandler(onExitWildMagicLumieresProtectricesAura);

      onRemoveWarMasterDesarmementCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveWarMasterDesarmement);

      onEnterPerceptionAveugleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterPerceptionAveugle);
    }
  }
}
