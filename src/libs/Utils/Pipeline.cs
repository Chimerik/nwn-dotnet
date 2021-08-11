using System;
using System.Linq;

namespace Utils
{
  public class Pipeline<T>
  {
    private T currentContext;
    private int currentIndex = 0;
    private Action<T, Action>[] middlewares;

    public Pipeline(Action<T, Action>[] middlewares)
    {
      this.middlewares = middlewares;
    }

    public void Execute(T context)
    {
      currentContext = context;
      currentIndex = 0;
      Next();
    }

    private void Next()
    {
      var middleware = middlewares.ElementAtOrDefault(currentIndex);
      currentIndex++;
      middleware?.Invoke(currentContext, Next);
    }
  }
}
