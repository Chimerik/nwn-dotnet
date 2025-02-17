using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect speakAnimalEffect
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.CommunicationAnimale);
        eff.Tag = SpeakAnimalEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.SpeakAnimal);
        return eff;
      }
    }
    public const string SpeakAnimalEffectTag = "_SPEAK_ANIMAL_EFFECT";
  }
}
