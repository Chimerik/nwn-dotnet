using System.Collections.Generic;

namespace NWN.Systems
{
  public class StartingPackage
  {
    public readonly List<Learnable> freeLearnables = new(); // + les 2 JDS maîtrisés
    public readonly List<Learnable> learnables = new();
    public readonly List<Learnable> skillChoiceList = new();
    public readonly int nbSkills;

    public StartingPackage(List<Learnable> freeLearnables, List<Learnable> learnables, List<Learnable> skillChoiceList, int nbSkills) 
    {
      this.freeLearnables = freeLearnables;
      this.learnables = learnables;
      this.skillChoiceList = skillChoiceList;
      this.nbSkills = nbSkills;
    }
  }
}
