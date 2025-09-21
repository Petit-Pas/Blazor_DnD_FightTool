using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDUserInteraction;

public interface IUserInteractionService
{
    event Func<IUserInteraction, Task>? InteractionRaised;

    Task<IQueryResponse<TResponse>> RequestAsync<TResponse>(IUserInteraction<TResponse> request);

    void CompleteSuccess<TResponse>(Guid id, TResponse response);

    void CompleteCanceled(Guid id);
}
