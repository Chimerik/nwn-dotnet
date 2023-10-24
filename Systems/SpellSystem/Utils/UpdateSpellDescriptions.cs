using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void UpdateSpellDescriptionTable()
    {
      foreach (var entry in Spells2da.spellTable)
        if (!string.IsNullOrEmpty(entry.googleDocId))
          UpdateSpellDescription(entry.tlkEntry, entry.googleDocId);
    }
    private static async void UpdateSpellDescription(StrRef description, string docId)
    {
      description.Override = await StringUtils.DownloadGoogleDoc(docId);
    }
  }
}
