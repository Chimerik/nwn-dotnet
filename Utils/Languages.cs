using Google.Cloud.Translation.V2;
using Anvil.API;

namespace NWN.Systems
{
  public static class Languages
  {
    public static string GetLangueStringConvertedHRPProtection(string sToConvert, int langue)
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
    private static string GetLanguageCode(int langue)
    {
      switch (langue)
      {
        case CustomSkill.Elfique:
          return LanguageCodes.Basque;
        case CustomSkill.Abyssal:
          return LanguageCodes.Latin;
        case CustomSkill.Celestial:
          return LanguageCodes.Swedish;
        case CustomSkill.Profond:
          return LanguageCodes.Welsh;
        case CustomSkill.Draconique:
          return LanguageCodes.Icelandic;
        case CustomSkill.Druidique:
          return LanguageCodes.Corsican;
        case CustomSkill.Nain:
          return LanguageCodes.German;
        case CustomSkill.Gnome:
          return LanguageCodes.Albanian;
        case CustomSkill.Giant:
          return LanguageCodes.Georgian;
        case CustomSkill.Gobelin:
          return LanguageCodes.Zulu;
        case CustomSkill.Halfelin:
          return LanguageCodes.Yoruba;
        case CustomSkill.Infernal:
          return LanguageCodes.Xhosa;
        case CustomSkill.Orc:
          return LanguageCodes.Uzbek;
        case CustomSkill.Primordiale:
          return LanguageCodes.Urdu;
        case CustomSkill.Sylvain:
          return LanguageCodes.Thai;
        case CustomSkill.Voleur:
          return LanguageCodes.Serbian;
      }

      return LanguageCodes.French;
    }
  }
}
