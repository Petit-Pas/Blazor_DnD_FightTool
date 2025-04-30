using Blazored.Modal.Services;

namespace FightBlazorComponents;

public class ModalServiceProvider : IModalServiceProvider
{
    private IModalService? _modalService;

    public void ConfigureModalService(IModalService modalService)
    {
        _modalService = modalService;
    }

    public IModalService? GetModalService()
    {
        return _modalService;
    }
}
