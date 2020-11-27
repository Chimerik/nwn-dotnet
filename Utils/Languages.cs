using Google.Cloud.Translation.V2;

namespace NWN.Systems
{
   public static class Languages
   {
     public static string GetLangueStringConvertedHRPProtection(string sToConvert, Feat langue)
     {
       if(sToConvert.Contains("("))
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
         case Feat.LanguageElf:
          return LanguageCodes.Basque;
        case Feat.LanguageAbyssal:
          return LanguageCodes.Latin;
        case Feat.LanguageCelestial:
          return LanguageCodes.Swedish;
        case Feat.LanguageDeep:
          return LanguageCodes.Welsh;
        case Feat.LanguageDraconic:
          return LanguageCodes.Icelandic;
        case Feat.LanguageDruidic:
          return LanguageCodes.Corsican;
        case Feat.LanguageDwarf:
          return LanguageCodes.German;
        case Feat.LanguageGnome:
          return LanguageCodes.Albanian;
        case Feat.LanguageGiant:
          return LanguageCodes.Georgian;
        case Feat.LanguageGoblin:
          return LanguageCodes.Zulu;
        case Feat.LanguageHalfling:
          return LanguageCodes.Yoruba;
        case Feat.LanguageInfernal:
          return LanguageCodes.Xhosa;
        case Feat.LanguageOrc:
          return LanguageCodes.Uzbek;
        case Feat.LanguagePrimodial:
          return LanguageCodes.Urdu;
        case Feat.LanguageSylvan:
          return LanguageCodes.Thai;
        case Feat.LanguageThieves:
          return LanguageCodes.Serbian;
      }

      return LanguageCodes.French;
    }
    public static string GetLanguageName(Feat langue)
    {
      switch (langue)
      {
        case Feat.LanguageElf:
          return "elfique";
        case Feat.LanguageAbyssal:
          return "abyssal";
        case Feat.LanguageCelestial:
          return "céleste";
        case Feat.LanguageDeep:
          return "outrelangue";
        case Feat.LanguageDraconic:
          return "draconique";
        case Feat.LanguageDruidic:
          return "druidique";
        case Feat.LanguageDwarf:
          return "nain";
        case Feat.LanguageGiant:
          return "géant";
        case Feat.LanguageGoblin:
          return "gobelin";
        case Feat.LanguageHalfling:
          return "halfelin";
        case Feat.LanguageInfernal:
          return "infernal";
        case Feat.LanguageOrc:
          return "orc";
        case Feat.LanguagePrimodial:
          return "primordial";
        case Feat.LanguageSylvan:
          return "sylvain";
        case Feat.LanguageThieves:
          return "langue des voleurs";
        case Feat.LanguageGnome:
          return "gnome";
      }

      return "commun";
    }
  }
}
