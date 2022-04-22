using System.Collections.Generic;

namespace NWN.Systems
{
    public static partial class CommandSystem
    {
        private static readonly Dictionary<string, Command> commandDic = new ()
    {
      /*{
        "help", new Command(
          name: "help",
          description: new Command.Description(
            title: "Affiche la liste de toutes les commandes disponibles.",
            examples: new string[]
            {
              ""
            }
          ),
          execute: ExecuteHelpCommand
        )
      },*/
      {
        "test",
        new Command(
          name: "test",
          description: new Command.Description(title: "Permet des tester des trucs."),
          execute: ExecuteTestCommand,
          options: new Options(
            positional: new List<Option>()
            {
              new Option(
                name: "skill",
                description: "Id du skill de test.",
                defaultValue: ""
              )
            }
          )
        )
      },
      {
        "dm",
        new Command(
          name: "dm",
          description: new Command.Description(
            title: "Affiche le menu dm."
          ),
          execute: ExecuteDMMenuCommand
        )
      }
    };
    }
}
