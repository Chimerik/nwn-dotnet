﻿using System;
using System.Numerics;
using Anvil.API;
using Anvil.Native;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void BroadcastNativeServerMessage(string message, CNWSCreature creature)
    {
      CExoLinkedListCNWSClient playerList = NWNXLib.AppManager().m_pServerExoApp.m_pcExoAppInternal.m_pNWSPlayerList;

      for (CExoLinkedListNode node = playerList.GetHeadPos(); node != null; node = node.pNext)
      {
        CNWSPlayer player = playerList.GetAtPos(node).AsNWSPlayer();

        if (player.m_oidNWSObject == creature.m_idSelf)
          continue;

        CNWSCreature playerCreature = NWNXLib.AppManager().m_pServerExoApp.GetCreatureByGameObjectID(player.m_oidNWSObject);

        if (playerCreature.m_oidArea != creature.m_oidArea
          || Vector3.DistanceSquared(playerCreature.m_vPosition.ToManagedVector(), creature.m_vPosition.ToManagedVector()) > 1225)
          continue;

        CNWCCMessageData pData = new CNWCCMessageData();
        pData.SetString(0, message.ToExoString());
        playerCreature.SendFeedbackMessage(204, pData);
        GC.SuppressFinalize(pData);
      }
    }
  }
}
