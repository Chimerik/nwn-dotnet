using Anvil.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static bool IsCannotThreatenEffect(EffectType eff) // TODO : Il manque l'effet Knockdown, peut-être à appliquer avec un tag ?
    {
      return eff switch
      {
        EffectType.Blindness or EffectType.Charmed or EffectType.Confused or EffectType.CutsceneImmobilize 
        or EffectType.CutsceneParalyze or EffectType.Darkness or EffectType.Dazed or EffectType.Dominated 
        or EffectType.Entangle or EffectType.Ethereal or EffectType.Frightened or EffectType.Pacify or EffectType.Paralyze 
        or EffectType.Petrify or EffectType.Sanctuary or EffectType.Sleep or EffectType.TimeStop or EffectType.Turned => true,
        _ => false,
      };
    }
  }
}
