using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PattesDaraigneeEffectTag = "_PATTES_DARAIGNEE_EFFECT";
    public static Effect PattesDaraignee
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.PattesDaraignee);
        eff.Tag = PattesDaraigneeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
