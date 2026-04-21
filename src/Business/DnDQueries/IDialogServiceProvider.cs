using MudBlazor;

namespace DnDFightTool.Business.DnDQueries;

public interface IDialogServiceProvider
{
    void SetDialogService(IDialogService dialogService);
    IDialogService GetDialogService();
}
