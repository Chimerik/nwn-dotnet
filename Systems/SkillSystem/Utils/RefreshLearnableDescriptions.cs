using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static void RefreshLearnableDescriptions()
    {
      UpdateSpellDescriptionTable();
    }

    public static async void UpdateSpellDescriptionTable()
    {
      ModuleSystem.Log.Info("Spell description started");
      int i = 0;
      foreach (var entry in Spells2da.spellTable)
      {
        if (!string.IsNullOrEmpty(entry.googleDocId))
        {
          if (i > 50)
          {
            await NwTask.Delay(TimeSpan.FromSeconds(5));
            i = 0;
          }

          UpdateSpellDescription(entry.tlkEntry, entry.googleDocId);
          i++;
        }
      }

      ModuleSystem.Log.Info("Spell description updated");
      UpdateLearnableDescriptions();
    }
    private static async void UpdateSpellDescription(StrRef description, string docId)
    {
      try
      {
        description.Override = await StringUtils.DownloadGoogleDoc(docId);
      }
      catch (Exception e)
      {
        LogUtils.LogMessage($"Echec chargement description des sorts :\n\n {e.Message}\n{e.StackTrace}", LogUtils.LogType.ModuleAdministration);
      }
    }

    private static async void UpdateLearnableDescriptions()
    {
      int i = 0;

      foreach (var learnable in learnableDictionary.Values)
      {
        if (!string.IsNullOrEmpty(learnable.descriptionLink))
        {
          if (i > 100)
          {
            await NwTask.Delay(TimeSpan.FromSeconds(5));
            i = 0;
          }

          UpdateDescriptionFromGoogleURL(learnable);
          i++;
        }
      }

      ModuleSystem.Log.Info("Learnable descriptions updated");
      StringUtils.InitializeTlkOverrides();
    }

    private static async void UpdateDescriptionFromGoogleURL(Learnable learnable)
    {
      try
      {
        learnable.description = await StringUtils.DownloadGoogleDoc(learnable.descriptionLink);
      }
      catch (Exception e)
      {
        LogUtils.LogMessage($"Echec chargement description des sorts :\n\n {e.Message}\n{e.StackTrace}", LogUtils.LogType.ModuleAdministration);
      }
    }
  }
}
