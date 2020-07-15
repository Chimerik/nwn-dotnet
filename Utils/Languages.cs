using System;
using System.Collections.Generic;
using System.Text;
using NWN.Enums;

namespace NWN
{
   public static class Languages
   {
     public static string GetLangueStringConvertedHRPProtection(string sToConvert, int iLangue)
     {
       if(sToConvert.Contains("("))
           return sToConvert;
       if(!sToConvert.Contains("*"))
           return ConvertStringToLangue(sToConvert, iLangue);

      string[] sArray = sToConvert.Split('*', '*');
      string sTranslated = "";
      int i = 0;

      foreach (string s in sArray)
      {
        if (i % 2 == 0)
          sTranslated += ConvertStringToLangue(s, iLangue);
        else
          sTranslated += $" * {s} * ";

        i++;
      }
      return sTranslated;
     }
     private static string ConvertStringToLangue(string sToConvert, int iLangue)
     {
       //if (iLangue == (int)Feat.LanguageThief && sToConvert.Length > 25)
       //{
        // sToConvert = sToConvert.Remove(25);
         //NWScript.SendMessageToPC(oPC, "Attention, la langue des voleurs ne part d'exprimer que de courtes idées.");
       //}

       //On vérifie que ce que t'as dit finit pas par un crochet
       if (sToConvert.EndsWith("]"))
         sToConvert = sToConvert.Remove(sToConvert.Length - 1);

       switch (iLangue)
       {
         case (int)Feat.LanguageElf:
           sToConvert = ConvertElven(sToConvert);
           break;
 /*        case _LANGUE_GNOME:
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertGnome(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_HALFELIN:
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertHalfelin(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_NAIN:
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertNain(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_ORC:
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertOrc(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_GOBELIN:
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertGobelin(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_DRACONIQUE: // Retour
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertDraconique(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_ANIMAL: // Suivant
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertAnimal(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_VOLEUR: // End
           if (GetStringLength(sToConvert) > 1)
             sToConvert = GetStringLeft(sToConvert, 1);

           if (sToConvert == "a" || sToConvert == "A") sOutput = "*se cache ses yeux*";
           else if (sToConvert == "b" || sToConvert == "B") sOutput = "*se gratte le bras*";
           else if (sToConvert == "c" || sToConvert == "C") sOutput = "*tousse*";
           else if (sToConvert == "d" || sToConvert == "D") sOutput = "*fronce les sourcils*";
           else if (sToConvert == "e" || sToConvert == "E") sOutput = "*regarde par terre*";
           else if (sToConvert == "f" || sToConvert == "F") sOutput = "*tourne la tête*";
           else if (sToConvert == "g" || sToConvert == "G") sOutput = "*regarde en l'air*";
           else if (sToConvert == "h" || sToConvert == "H") sOutput = "*fait craquer son cou*";
           else if (sToConvert == "i" || sToConvert == "I") sOutput = "*se masse les tempes*";
           else if (sToConvert == "j" || sToConvert == "J") sOutput = "*se frotte le menton*";
           else if (sToConvert == "k" || sToConvert == "K") sOutput = "*se gratte l'oreille*";
           else if (sToConvert == "l" || sToConvert == "L") sOutput = "*se retourne*";
           else if (sToConvert == "m" || sToConvert == "M") sOutput = "*sourire en coin*";
           else if (sToConvert == "n" || sToConvert == "N") sOutput = "*signe de tête*";
           else if (sToConvert == "o" || sToConvert == "O") sOutput = "*sourit*";
           else if (sToConvert == "p" || sToConvert == "P") sOutput = "*grimace*";
           else if (sToConvert == "q" || sToConvert == "Q") sOutput = "*frissonne*";
           else if (sToConvert == "r" || sToConvert == "R") sOutput = "*roule des yeux*";
           else if (sToConvert == "s" || sToConvert == "S") sOutput = "*se gratte le nez*";
           else if (sToConvert == "t" || sToConvert == "T") sOutput = "*se tourne un peu*";
           else if (sToConvert == "u" || sToConvert == "U") sOutput = "*regarde autour de lui*";
           else if (sToConvert == "v" || sToConvert == "V") sOutput = "*s'étire*";
           else if (sToConvert == "w" || sToConvert == "W") sOutput = "*fait des signes*";
           else if (sToConvert == "x" || sToConvert == "X") sOutput = "*éternue*";
           else if (sToConvert == "y" || sToConvert == "Y") sOutput = "*baille*";
           else if (sToConvert == "z" || sToConvert == "Z") sOutput = "*hausse les épaules*";
           else sOutput = "*signe de tête*";
           break;
         case _LANGUE_CELESTE: // End
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertCeleste(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_ABYSSAL: // End
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertAbyssal(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_INFERNAL: // End
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertInfernal(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_DROW: // End
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertDrow(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_AERIEN: // End
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertAerien(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
         case _LANGUE_SAHUAGIN: // End
           nCount = GetStringLength(sToConvert);
           while (nCount > 0)
           {
             if (GetStringLeft(sToConvert, 1) == "*")
             {
               iToggle = abs(iToggle - 1);
             }
             if (iToggle)
             {
               sOutput = sOutput + GetStringLeft(sToConvert, 1);
             }
             if (GetStringLeft(sToConvert, 1) == " ")
             {
               sOutput = sOutput + " ";
             }
             else
             {
               sOutput = sOutput + ConvertSahuagin(sToConvert);
             }
             sToConvert = GetStringRight(sToConvert, GetStringLength(sToConvert) - 1);
             nCount--;
           }
           break;
  */
}

      return sToConvert;
    }
    private static string ConvertElven(string sToConvert)
    {
      string sTranslate = "";
      
      foreach(char sLetter in sToConvert)
      {
        switch (sLetter)
        {
          case 'a':
            {
              sTranslate += "no";
              break;
            }
          case 'A':
            {
              sTranslate += "Nel";
              break;
            }
          case 'b':
            {
              sTranslate += "f";
              break;
            }
          case 'B':
            {
              sTranslate += "Fel";
              break;
            }
          case 'c':
            {
              sTranslate += "ny";
              break;
            }
          case 'C':
            {
              sTranslate += "Nyel";
              break;
            }
          case 'd':
            {
              sTranslate += "w";
              break;
            }
          case 'D':
            {
              sTranslate += "Wel";
              break;
            }
          case 'e':
            {
              sTranslate += "a";
              break;
            }
          case 'E':
            {
              sTranslate += "A";
              break;
            }
          case 'f':
            {
              sTranslate += "o";
              break;
            }
          case 'F':
            {
              sTranslate += "Oel";
              break;
            }
          case 'g':
            {
              sTranslate += "v";
              break;
            }
          case 'G':
            {
              sTranslate += "Vel";
              break;
            }
          case 'h':
            {
              sTranslate += "ir";
              break;
            }
          case 'H':
            {
              sTranslate += "Ir";
              break;
            }
          case 'i':
            {
              sTranslate += "e";
              break;
            }
          case 'I':
            {
              sTranslate += "E";
              break;
            }
          case 'j':
            {
              sTranslate += "qu";
              break;
            }
          case 'J':
            {
              sTranslate += "Quel";
              break;
            }
          case 'k':
            {
              sTranslate += "n";
              break;
            }
          case 'K':
            {
              sTranslate += "Nel";
              break;
            }
          case 'l':
            {
              sTranslate += "c";
              break;
            }
          case 'L':
            {
              sTranslate += "Cel";
              break;
            }
          case 'm':
            {
              sTranslate += "s";
              break;
            }
          case 'M':
            {
              sTranslate += "Sel";
              break;
            }
          case 'n':
            {
              sTranslate += "l";
              break;
            }
          case 'N':
            {
              sTranslate += "L";
              break;
            }
          case 'o':
            {
              sTranslate += "e";
              break;
            }
          case 'O':
            {
              sTranslate += "E";
              break;
            }
          case 'p':
            {
              sTranslate += "no";
              break;
            }
          case 'P':
            {
              sTranslate += "No";
              break;
            }
          case 'q':
            {
              sTranslate += "h";
              break;
            }
          case 'Q':
            {
              sTranslate += "H";
              break;
            }
          case 'r':
            {
              sTranslate += "m";
              break;
            }
          case 'R':
            {
              sTranslate += "Mel";
              break;
            }
          case 's':
            {
              sTranslate += "la";
              break;
            }
          case 'S':
            {
              sTranslate += "La";
              break;
            }
          case 't':
            {
              sTranslate += "an";
              break;
            }
          case 'T':
            {
              sTranslate += "An";
              break;
            }
          case 'u':
            {
              sTranslate += "y";
              break;
            }
          case 'U':
            {
              sTranslate += "Yel";
              break;
            }
          case 'v':
            {
              sTranslate += "el";
              break;
            }
          case 'V':
            {
              sTranslate += "El";
              break;
            }
          case 'w':
            {
              sTranslate += "am";
              break;
            }
          case 'W':
            {
              sTranslate += "Am";
              break;
            }
          case 'x':
            {
              sTranslate += "'";
              break;
            }
          case 'X':
            {
              sTranslate += "'";
              break;
            }
          case 'y':
            {
              sTranslate += "a";
              break;
            }
          case 'Y':
            {
              sTranslate += "Ael";
              break;
            }
          case 'z':
            {
              sTranslate += "j";
              break;
            }
          case 'Z':
            {
              sTranslate += "Jel";
              break;
            }
          case '?':
            {
              sTranslate += "?";
              break;
            }
          case '!':
            {
              sTranslate += "!";
              break;
            }
          case ';':
            {
              sTranslate += ";";
              break;
            }
          case ',':
            {
              sTranslate += ",";
              break;
            }
          case '.':
            {
              sTranslate += ".";
              break;
            }
          case ' ':
            {
              sTranslate += " ";
              break;
            }
          default:
          {
              sTranslate += "el";
              break;
          }
        }
      }
      
      return sTranslate;
    }
  }
}
