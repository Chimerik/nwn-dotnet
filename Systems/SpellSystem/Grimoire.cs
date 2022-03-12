using System.Collections.Generic;

namespace NWN.Systems
{
  public class Grimoire
  {
    public string name { get; set; }
    public List<int> spellList { get; set; }
    public List<int> metamagicList { get; set; }

    public Grimoire(string name, List<int> spellList, List<int> metamagicList)
    {
      this.name = name;
      this.spellList = spellList;
      this.metamagicList = metamagicList;
    }
    public Grimoire()
    {

    }
  }
}
