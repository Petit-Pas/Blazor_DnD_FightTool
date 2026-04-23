using DnDFightTool.Business.DnDQueries;
using MudBlazor;

namespace DnDFightTool.UI.DnDQueryPrompter;

public class DialogServiceProvider : IDialogServiceProvider
{
    private IDialogService _dialogService = null!;

    public DialogServiceProvider()
    {
    }

    public IDialogService GetDialogService()
    {
        return _dialogService ?? throw new NullReferenceException("Should have set dialog service first.");
    }

    public void SetDialogService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
}
