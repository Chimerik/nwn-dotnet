using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NLog;

namespace NWN.Systems.Alchemy
{
  [ServiceBinding(typeof(AlchemySystem))]
  class AlchemySystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static string[,] alchemyTable;
    public static Vector2 center;
    public AlchemySystem()
    {
      /*foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("alchemy_cauldron"))
        plc.OnUsed += StartAlchemyTableDialog;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("alchemy_mortar"))
        plc.OnUsed += StartAlchemyMortarDialog;*/

      //LoadAlchemyTable();
    }

    public static void StartAlchemyTableDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (PlayerSystem.Players.TryGetValue(onUsed.UsedBy, out PlayerSystem.Player player))
        new AlchemyTableDialog(player, onUsed.Placeable);
    }
    public static void StartAlchemyMortarDialog(PlaceableEvents.OnUsed onUsed)
    {
      if (PlayerSystem.Players.TryGetValue(onUsed.UsedBy, out PlayerSystem.Player player))
        new AlchemyMortarDialog(player);
    }
    public void LoadAlchemyTable()
    {
      try
      {
        var path = "/home/chim/test.csv";
        var lines = File.ReadLines(path, Encoding.UTF8);

        int rows = lines.Count();
        int nbColumns = lines.FirstOrDefault().Count(c => c == ',') + 1;

        if (rows < 1)
          return;

        alchemyTable = new string[rows, nbColumns];
        
        int x = 0;

        foreach (var line in lines)
        {
          var columns = line.Split(",");
          int y = 0;

          foreach (var column in columns)
          {
            alchemyTable[x, y] = column;

            if (column == "START")
              center = new Vector2(x, y);

            y++;
          }

          x++;
        }
      }
      catch(Exception e)
      {
        Utils.LogMessageToDMs($"WARNING - Could not load AlchemyTable : {e.Message}");
      }

      /*Log.Info("------------------------------------------------------------");

      for (int i = 0; i < rows; i++)
      {
        string output = "";

        for (int j = 0; j < nbColumns; j++)
          output += alchemyTable[i, j] + ",";

        Log.Info(output);
      }

      Log.Info("------------------------------------------------------------");*/
    }
    /*public static void AddAlchemyProperties(NwItem item, string serializedProperties)
    {
      string[] json = serializedProperties.Split("|");

      CustomUnpackedEffect customUnpackedEffect = JsonConvert.DeserializeObject<CustomUnpackedEffect>(json);
      customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(player.oid.ControlledCreature);
    }*/
  }
}
