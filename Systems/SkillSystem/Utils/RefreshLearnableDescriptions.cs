using System.Linq;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static void RefreshLearnableDescriptions()
    {
      StringUtils.InitializeTlkOverrides();
      SpellUtils.UpdateSpellDescriptionTable();

      foreach (var learnable in learnableDictionary.Values.Where(l => !string.IsNullOrEmpty(l.descriptionLink)))
        UpdateDescriptionFromGoogleURL(learnable);
    }
    private static async void UpdateDescriptionFromGoogleURL(Learnable learnable)
    {
      learnable.description = await StringUtils.DownloadGoogleDoc(learnable.descriptionLink);
    }
  }
}
