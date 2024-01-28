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

      onEnterChanceDebordanteCallback = scriptHandleFactory.CreateUniqueHandler(onEnterChanceDebordante);
      onExitChanceDebordanteCallback = scriptHandleFactory.CreateUniqueHandler(onExitChanceDebordante);

      onRemoveAgressionOrcCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveAgressionOrc);

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

      onIntervalBarbarianRageCallback = scriptHandleFactory.CreateUniqueHandler(OnIntervalBarbarianRage);
      onRemoveBarbarianRageCallback = scriptHandleFactory.CreateUniqueHandler(OnRemoveBarbarianRage);
    }
  }
}
