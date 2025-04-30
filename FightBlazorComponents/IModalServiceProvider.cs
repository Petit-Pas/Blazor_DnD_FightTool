using Blazored.Modal.Services;

namespace FightBlazorComponents;

public interface IModalServiceProvider
{
    IModalService? GetModalService();

    void ConfigureModalService(IModalService modalService);
}
