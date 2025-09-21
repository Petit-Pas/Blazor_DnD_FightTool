namespace DnDFightTool.Business.DnDUserInteraction;

public interface IUserInteraction<TAnswer> : IUserInteraction
{
}

public interface IUserInteraction
{
    Guid InteractionId { get; }
}
