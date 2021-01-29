namespace NWN.Systems
{
  class ScriptPerf
  {
    public int nbExecution { get; set; }
    public double cumulatedExecutionTime { get; set; }
    public ScriptPerf(double executionTime)
    {
      nbExecution = 1;
      cumulatedExecutionTime = executionTime;
    }
  }
}
