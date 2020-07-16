using NWN.Enums;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFrostAttackCommand(ChatSystem.ChatEventArgs e, Options.Result options)
    {
      if (
        NWScript.GetLevelByClass(ClassType.Wizard, e.oSender) > 0 ||
        NWScript.GetLevelByClass(ClassType.Sorcerer, e.oSender) > 0
      )
      {
        if (NWNX.Object.GetInt(e.oSender, "_FROST_ATTACK") == 0)
        {
          NWNX.Object.SetInt(e.oSender, "_FROST_ATTACK", 1, true);
          NWScript.SendMessageToPC(e.oSender, "Vous activez le mode d'attaque par rayon de froid");
        }
        else
        {
          NWNX.Object.DeleteInt(e.oSender, "_FROST_ATTACK");
          NWScript.SendMessageToPC(e.oSender, "Vous désactivez le mode d'attaque par rayon de froid");
        }
      }
      else
        NWScript.SendMessageToPC(e.oSender, "Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");
    }
  }
}
