using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CanPrepareSpellsEffectTag = "_CAN_PREPARE_SPELLS_EFFECT";
    public static Effect CanPrepareSpells
    {
      get
      {
        Effect eff =  Effect.RunAction();
        eff.Tag = CanPrepareSpellsEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
