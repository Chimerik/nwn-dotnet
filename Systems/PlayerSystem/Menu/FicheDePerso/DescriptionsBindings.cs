using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public partial class FicheDePersoWindow : PlayerWindow
      {
        private void DescriptionsBindings()
        {
          resistanceRow.Children.Clear();

          description.SetBindValue(player.oid, nuiToken.Token, player.oid.LoginCreature.Description);
          SetDescriptionListBindings(true);
        }

        private void SetDescriptionListBindings(bool setTitle = false)
        {
          List<string> titleList = new();

          foreach (CharacterDescription description in player.descriptions)
          {
            titleList.Add(description.name);

            if (setTitle && description.description == player.oid.LoginCreature.Description)
              title.SetBindValue(player.oid, nuiToken.Token, description.name);
          }

          descriptionTitles.SetBindValues(player.oid, nuiToken.Token, titleList);
          descriptionsListCount.SetBindValue(player.oid, nuiToken.Token, titleList.Count);
        }
      }
    }
  }
}
