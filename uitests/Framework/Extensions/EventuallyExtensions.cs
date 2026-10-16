namespace DnDFightTool.UiTests.Framework.Extensions;

/// <summary>
///     Retries an assertion against a real service until it passes, for effects the browser triggers asynchronously.
/// </summary>
public static class EventuallyExtensions
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    /// <summary>
    ///     Runs <paramref name="assertion"/> repeatedly until it stops throwing or the timeout elapses.
    /// </summary>
    /// <typeparam name="T">The type of the subject under assertion.</typeparam>
    /// <param name="subject">The service whose state is asserted.</param>
    /// <param name="assertion">The assertion to retry, typically a FluentAssertions chain.</param>
    /// <param name="timeout">The maximum time to wait; defaults to five seconds.</param>
    /// <exception cref="Exception">The last assertion failure, once the timeout has elapsed.</exception>
    public static async Task Eventually<T>(this T subject, Action<T> assertion, TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? DefaultTimeout);
        while (true)
        {
            try
            {
                assertion(subject);
                return;
            }
            catch (Exception) when (DateTime.UtcNow < deadline)
            {
            }

            await Task.Delay(PollInterval);
        }
    }
}
