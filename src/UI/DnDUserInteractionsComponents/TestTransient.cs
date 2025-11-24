using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDQueries;
using FightBlazorComponents.Entities.MartialAttacks;
using MudBlazor;

namespace DnDUserInteractionsComponents;

public class TestTransient : ITestTransient
{
    private readonly IDialogService _dialogService;

    public TestTransient(IDialogService dialogService)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    public Task TestAsync()
    {
        var areTheSame = _dialogService == MartialAttackSelectorComponent.SingletonDialogService;
        return Task.CompletedTask;
    }
}
