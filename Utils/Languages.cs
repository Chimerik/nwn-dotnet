using Google.Cloud.Translation.V2;

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
      //if (iLangue == (int)Feat.LanguageThief && sToConvert.Length > 25)
      //{
      // sToConvert = sToConvert.Remove(25);
      //NWScript.SendMessageToPC(oPC, "Attention, la langue des voleurs ne part d'exprimer que de courtes idées.");
      //}

      switch (langue)
      {
        case Feat.Elfique:
          return LanguageCodes.Basque;
        case Feat.Abyssal:
          return LanguageCodes.Latin;
        case Feat.Céleste:
          return LanguageCodes.Swedish;
        case Feat.Profond:
          return LanguageCodes.Welsh;
        case Feat.Draconique:
          return LanguageCodes.Icelandic;
        case Feat.Druidique:
          return LanguageCodes.Corsican;
        case Feat.Nain:
          return LanguageCodes.German;
        case Feat.Gnome:
          return LanguageCodes.Albanian;
        case Feat.Géant:
          return LanguageCodes.Georgian;
        case Feat.Gobelin:
          return LanguageCodes.Zulu;
        case Feat.Halfelin:
          return LanguageCodes.Yoruba;
        case Feat.Infernal:
          return LanguageCodes.Xhosa;
        case Feat.Orc:
          return LanguageCodes.Uzbek;
        case Feat.Primordial:
          return LanguageCodes.Urdu;
        case Feat.Sylvain:
          return LanguageCodes.Thai;
        case Feat.Voleur:
          return LanguageCodes.Serbian;
      }

      return LanguageCodes.French;
    }
  }
}
