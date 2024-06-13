
namespace Spice.Extension;
    

public static class AsyncEnumerableExtensions
{
    public static IEnumerable<T> ToBlockingEnumerable<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        var enumerator = asyncEnumerable.GetAsyncEnumerator();
        try
        {
            while (true)
            {
                var moveNextTask = Task.Run(() => enumerator.MoveNextAsync().AsTask());
                if (!moveNextTask.GetAwaiter().GetResult()) break;
                yield return enumerator.Current;
            }
        }
        finally
        {
            Task.Run(() => enumerator.DisposeAsync()).GetAwaiter().GetResult();
        }
    }
}