using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(ExamineSystem))]
  public class ExamineSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static void OnExamineBefore(OnExamineObject onExamine)
    {
      Log.Info($"{onExamine.ExaminedBy.LoginCreature.Name} examining {onExamine.ExaminedObject.Name} - Tag : {onExamine.ExaminedObject.Tag}");

      if (!PlayerSystem.Players.TryGetValue(onExamine.ExaminedBy.LoginCreature, out PlayerSystem.Player player))
        return;

      if(onExamine.ExaminedObject is NwItem item)
      {
        // TODO : annuler l'ouverture de la fenêtre examiner habituelle

        if (player.windows.ContainsKey("itemExamine"))
          ((PlayerSystem.Player.ItemExamineWindow)player.windows["itemExamine"]).CreateWindow(item);
        else
          player.windows.Add("itemExamine", new PlayerSystem.Player.ItemExamineWindow(player, item));

        return;
      }
      
      switch (onExamine.ExaminedObject.Tag)
      {
        case "mineable_materia":

          if (player.windows.ContainsKey("materiaExamine"))
            ((PlayerSystem.Player.MateriaExamineWindow)player.windows["materiaExamine"]).CreateWindow((NwPlaceable)onExamine.ExaminedObject);
          else
            player.windows.Add("materiaExamine", new PlayerSystem.Player.MateriaExamineWindow(player, (NwPlaceable)onExamine.ExaminedObject));

          break;
        case "refinery":
          string descriptionBrut = "Stock actuel de minerai brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.oresDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionBrut += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionBrut;
          break;
        case "forge":
          string descriptionRefined = "Stock actuel de minerai raffiné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.mineralDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionRefined += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionRefined;
          break;
        case "decoupe":
          string descriptionWood = "Stock actuel de bois brut : \n\n\n";
          foreach (var entry in Craft.Collect.Config.woodDictionnary)
            if (player.materialStock.TryGetValue(entry.Value.name, out int playerStock))
              descriptionWood += $"* {entry.Value.name}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionWood;
          break;
        case "scierie":
          string descriptionPlank = "Stock actuel de planches de bois raffinées : \n\n\n";
          foreach (var entry in Craft.Collect.Config.plankDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionPlank += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionPlank;
          break;
        case "tannerie_peau":
          string descriptionPelt = "Stock actuel de peaux brutes : \n\n\n";
          foreach (var entry in Craft.Collect.Config.peltDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionPelt += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionPelt;
          break;

        case "tannerie":
          string descriptionLeather = "Stock actuel de cuir tanné : \n\n\n";
          foreach (var entry in Craft.Collect.Config.leatherDictionnary)
            if (player.materialStock.TryGetValue(entry.Key.ToString(), out int playerStock))
              descriptionLeather += $"* {entry.Key.ToDescription()}: {playerStock}\n\n";

          onExamine.ExaminedObject.Description = descriptionLeather;
          break;
      }
    }
  }
}
