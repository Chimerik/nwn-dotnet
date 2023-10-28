using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(EffectSystem))]
  public partial class EffectSystem
  {

    public static readonly string frightenedEffectTag = "FRIGHTENED_";
    public static readonly Native.API.CExoString frightenedEffectExoTag = "FRIGHTENED_".ToExoString();
    public static readonly Native.API.CExoString exoDelimiter = "_".ToExoString();

    private static ScriptHandleFactory scriptHandleFactory;

    public EffectSystem(ScriptHandleFactory scriptFactory)
    {
      scriptHandleFactory = scriptFactory;
    }
  }
}
