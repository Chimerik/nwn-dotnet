using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class ModuleSystem
  {
    private async void DailyReboot()
    {
      LogUtils.LogMessage($"Début de la séquence de reboot quotidienne", LogUtils.LogType.ModuleAdministration);

      foreach (NwPlayer connectingPlayer in NwModule.Instance.Players.Where(p => p.LoginCreature == null))
        connectingPlayer.BootPlayer("Navré, le module est en cours de redémarrage. Vous pourrez vous reconnecter dans une minute.");

      foreach (Player connectedPlayer in Players.Values.Where(p => p.pcState != PcState.Offline))
        connectedPlayer.windows.Add("rebootCountdown", new RebootCountdownWindow(connectedPlayer));

      await NwTask.Delay(TimeSpan.FromSeconds(31));

      SqLiteUtils.UpdateQuery("moduleInfo",
        new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } },
        new List<string[]>() { new string[] { "rowid", "1" } });

      TradeSystem.SaveToDatabase();

      await NwTask.Delay(TimeSpan.FromSeconds(4));
      NwServer.Instance.ShutdownServer();
    }
  }
}
