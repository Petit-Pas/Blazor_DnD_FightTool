using DnDFightTool.Business.DnDQueries;
using FightBlazorComponents.Entities.MartialAttacks;
using MudBlazor;

namespace DnDUserInteractionsComponents
{
    public class TestSingleton : ITestSingleton
    {
        private readonly IDialogService _dialogService;

        public TestSingleton(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public Task TestAsync()
        {
            var areTheSame = _dialogService == MartialAttackSelectorComponent.SingletonDialogService;
            return Task.CompletedTask;
        }
    }
}
