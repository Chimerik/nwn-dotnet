using NWN.Enums;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteFrostAttackCommand(ChatSystem.Context chatContext, Options.Result options)
    {
      if (
        NWScript.GetLevelByClass(ClassType.Wizard, chatContext.oSender) > 0 ||
        NWScript.GetLevelByClass(ClassType.Sorcerer, chatContext.oSender) > 0
      )
      {
        if (NWNX.Object.GetInt(chatContext.oSender, "_FROST_ATTACK") == 0)
        {
          NWNX.Object.SetInt(chatContext.oSender, "_FROST_ATTACK", 1, true);
          NWScript.SendMessageToPC(chatContext.oSender, "Vous activez le mode d'attaque par rayon de froid");
        }
        else
        {
          NWNX.Object.DeleteInt(chatContext.oSender, "_FROST_ATTACK");
          NWScript.SendMessageToPC(chatContext.oSender, "Vous désactivez le mode d'attaque par rayon de froid");
        }
      }
      else
        NWScript.SendMessageToPC(chatContext.oSender, "Il vous faut pouvoir lancer le sort rayon de froid pour activer ce mode.");
    }
  }
}
