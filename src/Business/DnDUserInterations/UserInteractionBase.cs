namespace DnDFightTool.Business.DnDUserInteraction;

public record UserInteractionBase<TAnswer> : IUserInteraction<TAnswer>
{
    public Guid InteractionId { get; } = Guid.NewGuid();
}
