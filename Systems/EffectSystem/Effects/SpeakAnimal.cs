using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect speakAnimalEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(143));
        eff.Tag = SpeakAnimalEffectTag;
        return eff;
      }
    }
    public const string SpeakAnimalEffectTag = "_SPEAK_ANIMAL_EFFECT";
  }
}
