using Google.Cloud.Translation.V2;
using NWN.API.Constants;

namespace NWN.Systems
{
  public static class Languages
  {
    public static string GetLangueStringConvertedHRPProtection(string sToConvert, Feat langue)
    {
      if (sToConvert.Contains("("))
        return sToConvert;

      string languageCode = GetLanguageCode(langue);

      if (!sToConvert.Contains("*"))
        return ModuleSystem.googleTranslationClient.TranslateText(sToConvert, languageCode).TranslatedText;

      string[] sArray = sToConvert.Split('*', '*');
      string sTranslated = "";
      int i = 0;

      foreach (string s in sArray)
      {
        if (i % 2 == 0)
          sTranslated += ModuleSystem.googleTranslationClient.TranslateText(s, languageCode).TranslatedText;
        else
          sTranslated += $" * {s} * ";

        i++;
      }
      return sTranslated;
    }
    private static string GetLanguageCode(Feat langue)
    {
      switch (langue)
      {
        case CustomFeats.Elfique:
          return LanguageCodes.Basque;
        case CustomFeats.Abyssal:
          return LanguageCodes.Latin;
        case CustomFeats.Céleste:
          return LanguageCodes.Swedish;
        case CustomFeats.Profond:
          return LanguageCodes.Welsh;
        case CustomFeats.Draconique:
          return LanguageCodes.Icelandic;
        case CustomFeats.Druidique:
          return LanguageCodes.Corsican;
        case CustomFeats.Nain:
          return LanguageCodes.German;
        case CustomFeats.Gnome:
          return LanguageCodes.Albanian;
        case CustomFeats.Géant:
          return LanguageCodes.Georgian;
        case CustomFeats.Gobelin:
          return LanguageCodes.Zulu;
        case CustomFeats.Halfelin:
          return LanguageCodes.Yoruba;
        case CustomFeats.Infernal:
          return LanguageCodes.Xhosa;
        case CustomFeats.Orc:
          return LanguageCodes.Uzbek;
        case CustomFeats.Primordiale:
          return LanguageCodes.Urdu;
        case CustomFeats.Sylvain:
          return LanguageCodes.Thai;
        case CustomFeats.Voleur:
          return LanguageCodes.Serbian;
      }

      return LanguageCodes.French;
    }
  }
}
