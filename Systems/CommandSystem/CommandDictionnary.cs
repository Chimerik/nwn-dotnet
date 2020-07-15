using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static Dictionary<string, Command> commandDic = new Dictionary<string, Command>
    {
      {
        "help", new Command(
          name: "help",
          description: new Command.Description(title: "Display the list of all available commands."),
          execute: ExecuteHelpCommand
        )
      },
      {
        "frostattack",
        new Command(
          name: "frostattack",
          description: new Command.Description(title: "todo"),
          execute: ExecuteFrostAttackCommand
        )
      },
      {
        "walk",
        new Command(
          name: "walk",
          description: new Command.Description(title: "todo"),
          execute: ExecuteWalkCommand
        )
      },
    };
  }
}
