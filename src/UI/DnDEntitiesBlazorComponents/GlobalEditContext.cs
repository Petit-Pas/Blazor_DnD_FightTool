using AspNetCoreExtensions.Navigations;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDEntitiesBlazorComponents;


/// <inheritdoc />
internal class GlobalEditContext : IGlobalEditContext
{
    /// NavigationManager wrapper
    private readonly IStateFullNavigation _stateFullNavigation;
    private readonly ICharacterRepository _characterRepository;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="stateFullNavigation"></param>
    /// <param name="characterRepository"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlobalEditContext(IStateFullNavigation stateFullNavigation, ICharacterRepository characterRepository)
    {
        _stateFullNavigation = stateFullNavigation ?? throw new ArgumentNullException(nameof(stateFullNavigation));
        _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
    }


    #region Character 
    
    /// <inheritdoc />
    public Character? Character { get; private set; }

    /// <inheritdoc />
    public void EditCharacter(Character character)
    {
        Character = character;
        _stateFullNavigation.NavigateTo("Characters/Edit");
    }

    /// <inheritdoc />
    public void CancelCharacterEdittion()
    {
        Character = null;
        _stateFullNavigation.NavigateBack();
    }

    /// <inheritdoc />
    public void SaveEditedCharacter()
    {
        if (Character != null)
        {
            _characterRepository.Save(Character);
        }
        Character = null;
        _stateFullNavigation.NavigateBack();
    }

    #endregion Character

    #region Attack 

    /// <inheritdoc />
    public MartialAttackTemplate? Attack { get; private set; }

    /// <inheritdoc />
    public void EditAttack(MartialAttackTemplate attack)
    {
        Attack = attack;
        _stateFullNavigation.NavigateTo("Attacks/Edit");
    }

    /// <inheritdoc />
    public void CancelAttackEdition()
    {
        Attack = null;
        _stateFullNavigation.NavigateBack();
    }

    /// <inheritdoc />
    public void SaveEditedAttack()
    {
        if (Character is not null && Attack is not null)
        {
            Character?.MartialAttacks.Add(Attack);
            _stateFullNavigation.NavigateBack();
        }
    }

    #endregion Attack
}
