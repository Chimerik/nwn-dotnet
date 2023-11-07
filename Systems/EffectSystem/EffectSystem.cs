using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(EffectSystem))]
  public partial class EffectSystem
  {

    public static readonly string frightenedEffectTag = "_FRIGHTENED_EFFECT";
    public static readonly Native.API.CExoString frightenedEffectExoTag = "_FRIGHTENED_EFFECT".ToExoString();
    public static readonly Native.API.CExoString exoDelimiter = "_".ToExoString();

    private static ScriptHandleFactory scriptHandleFactory;

    public EffectSystem(ScriptHandleFactory scriptFactory)
    {
      scriptHandleFactory = scriptFactory;
      onEnterProtectionStyleCallback = scriptHandleFactory.CreateUniqueHandler(onEnterProtectionStyle);
      onExitProtectionStyleCallback = scriptHandleFactory.CreateUniqueHandler(onExitProtectionStyle);
    }
  }
}
