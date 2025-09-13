using AspNetCoreExtensions.Navigations;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;

namespace DnDEntitiesBlazorComponents;


/// <inheritdoc />
internal class GlobalEditContext : IGlobalEditContext
{
    /// NavigationManager wrapper
    private readonly IStateFullNavigation _stateFullNavigation;
    private readonly ICharacterRepository _characterRepository;
    private readonly IFightContext _fightContext;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="stateFullNavigation"></param>
    /// <param name="characterRepository"></param>
    /// <param name="fightContext"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlobalEditContext(
        IStateFullNavigation stateFullNavigation, 
        ICharacterRepository characterRepository,
        IFightContext fightContext)
    {
        _stateFullNavigation = stateFullNavigation ?? throw new ArgumentNullException(nameof(stateFullNavigation));
        _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
        _fightContext = fightContext ?? throw new ArgumentNullException(nameof(fightContext));
    }


    #region Character 
    
    /// <inheritdoc />
    public ICharacter? Character { get; private set; }

    /// <inheritdoc />
    public void EditCharacter(ICharacter character)
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
            if (Character is FightingCharacter fighter)
            {
                _fightContext.Update(fighter);
            }
            else if (Character is Character character)
            {
                _characterRepository.Save(character);
            }
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
