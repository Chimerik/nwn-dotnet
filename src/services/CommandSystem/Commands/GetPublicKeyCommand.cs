using Anvil.API;

namespace NWN.Systems
{
  class GetPublicKey
  {
    public GetPublicKey(NwPlayer oPC)
    {
      oPC.SendServerMessage($"Votre clef publique est : {oPC.CDKey.ColorString(ColorConstants.White)}", ColorConstants.Pink);
    }
  }
}
