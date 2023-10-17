using System;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void SendNativeServerMessage(string message, CNWSCreature creature)
    {
      if (creature.m_bPlayerCharacter < 1)
        return;

      CNWCCMessageData pData = new CNWCCMessageData();
      pData.SetString(0, message.ToExoString());
      creature.SendFeedbackMessage(204, pData);
      GC.SuppressFinalize(pData);
    }
  }
}
