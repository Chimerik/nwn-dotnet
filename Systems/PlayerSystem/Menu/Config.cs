namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public abstract partial class Menu
    {
      private static class Config
      {
        public static class Font
        {
          public const string Text = "fnt_galahad14";
          public const string Gui = "fnt_es_gui";
        }

        public static class Glyph
        {
          public const string WindowTopLeft = "a";
          public const string WindowTopRight = "c";
          public const string WindowTopMiddle = "b";
          public const string WindowMiddleLeft = "d";
          public const string WindowMiddleRight = "f";
          public const string WindowMiddleBlank = "i";
          public const string WindowBottomLeft = "h";
          public const string WindowBottomRight = "g";
          public const string WindowBottomMiddle = "e";
          public const string Arrow = "j";
          public const string BlankWhite = "k";
        }

        public static class Color
        {
          public const uint Transparent = 0xFFFFFF00;
          public const uint White = 0xFFFFFFFF;
          public const uint Silver = 0xC0C0C0FF;
          public const uint Gray = 0x808080FF;
          public const uint DarkGray = 0x303030FF;
          public const uint Black = 0x000000FF;
          public const uint Red = 0xFF0000FF;
          public const uint Maroon = 0x800000FF;
          public const uint Orange = 0xFFA500FF;
          public const uint Yellow = 0xFFFF00FF;
          public const uint Olive = 0x808000FF;
          public const uint Lime = 0x00FF00FF;
          public const uint Green = 0x008000FF;
          public const uint Aqua = 0x00FFFFFF;
          public const uint Teal = 0x008080FF;
          public const uint Blue = 0x0000FFFF;
          public const uint Navy = 0x000080FF;
          public const uint Fuschia = 0xFF00FFFF;
          public const uint Purple = 0x800080FF;
        }
      }
    }
  } 
}
