using System.Collections.Concurrent;
using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDUserInteraction;

internal class UserInteractionService : IUserInteractionService
{
    public event Func<IUserInteraction, Task>? InteractionRaised;

    // We need to be able to find back the tasks that get completed, so we store them in a dictionary
    // Since this instance is not tied to the generic call, we are forced to use object here
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object>> _pending = new();

    public Task<IQueryResponse<TResponse>> RequestAsync<TResponse>(IUserInteraction<TResponse> request)
    {
        if (request is null)
        {
            return Task.FromResult(QueryResponse<TResponse>.Failed(default!));
        }

        // Create a custom task
        var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_pending.TryAdd(request.InteractionId, taskCompletionSource))
        {
            throw new InvalidOperationException("Duplicate interaction Id.");
        }

        // Fire & forget notify UI host
        InteractionRaised?.Invoke(request);

        // When the tasks (which returns TResponse) completes, wraps it in QueryResponse<TResponse>
        return taskCompletionSource.Task.ContinueWith(t =>
        {
            if (t.Exception?.InnerExceptions.Any(x => x.GetType() == typeof(OperationCanceledException)) ?? false)
            {
                return QueryResponse<TResponse>.Canceled(default!);
            }
            return QueryResponse<TResponse>.Success((TResponse)t.Result);
        });
    }

    public void CompleteSuccess<TResponse>(Guid id, TResponse response)
    {
        Complete(id, result: response);
    }

    public void CompleteCanceled(Guid id)
    {
        Complete(id, result: new OperationCanceledException());
    }

    private void Complete(Guid id, object? result)
    {
        if (_pending.TryRemove(id, out var tcs))
        {
            switch (result)
            {
                case OperationCanceledException oce: tcs.TrySetException(oce); break;
                default: tcs.TrySetResult(result!); break;
            }
        }
    }
}
