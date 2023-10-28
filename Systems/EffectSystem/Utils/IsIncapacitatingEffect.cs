using Anvil.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static bool IsIncapacitatingEffect(EffectType eff) // TODO : Il manque l'effet Knockdown, peut-être à appliquer avec un tag ?
    {
      return eff switch
      {
        EffectType.Charmed or EffectType.Confused or EffectType.CutsceneImmobilize 
        or EffectType.CutsceneParalyze or EffectType.Dazed or EffectType.Dominated
        or EffectType.Ethereal or EffectType.Paralyze or EffectType.Stunned
        or EffectType.Petrify or EffectType.Sanctuary or EffectType.Sleep or EffectType.Turned => true,
        _ => false,
      };
    }
  }
}
