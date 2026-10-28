using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.MartialAttacks.Pages;
using DomainTestsUtilities.Factories.Characters;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Factories.Dices;
using DomainTestsUtilities.Factories.MartialAttacks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.MartialAttacks.Components;

/// <summary>
///     To-hit modifier editing in the attack editor's "Main Informations" panel.
/// </summary>
public static class AttackMainInfoEditorUiTests
{
    /// <summary>
    ///     Rendering to-hit modifiers seeded through the repository.
    /// </summary>
    public sealed class DisplayToHitModifiers : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableCharacterListEditorPage _characterList = null!;

        /// <summary>
        ///     Resolves the repository and builds the list page object.
        /// </summary>
        [SetUp]
        public void SetUpCharacterLibrary()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            _characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
        }

        /// <summary>
        ///     The attack editor renders the to-hit modifiers stored on the attack.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_ToHit_Modifiers()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                modifiers: ModifiersTemplateFactory.Build("WIS+8")));
            _repository.Save(fighter);
            await _characterList.GoToAsync();
            var editor = await _characterList.EditPlayer("Fighter");
            var attacks = await editor.OpenAttacks();

            // Act
            var attackEditor = await attacks.EditAttack("Longsword");

            // Assert
            (await attackEditor.MainInfo().GetToHitModifiers()).Should().Be("WIS+8");
        }
    }

    /// <summary>
    ///     Saving an attack whose stored to-hit modifier is valid for modifiers but not for dice.
    /// </summary>
    public sealed class SaveUnchangedToHitModifiers : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableCharacterListEditorPage _characterList = null!;

        /// <summary>
        ///     Resolves the repository and builds the list page object.
        /// </summary>
        [SetUp]
        public void SetUpCharacterLibrary()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            _characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
        }

        /// <summary>
        ///     A stored negative to-hit modifier does not block Save when the user changes nothing.
        /// </summary>
        [Test]
        public async Task Should_Save_An_Unchanged_Negative_ToHit_Modifier()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                modifiers: ModifiersTemplateFactory.Build("-1")));
            _repository.Save(fighter);
            await _characterList.GoToAsync();
            var editor = await _characterList.EditPlayer("Fighter");
            var attacks = await editor.OpenAttacks();
            var attackEditor = await attacks.EditAttack("Longsword");

            // Act
            await CaptureAsync("before-save");
            editor = await attackEditor.Save();
            await editor.Save();

            // Assert
            StoredAttack(_repository).ToHitModifiers.Expression.Should().Be("-1");
        }
    }

    /// <summary>
    ///     Saving, refusing and discarding to-hit modifier changes.
    /// </summary>
    public sealed class EditToHitModifiers : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableCharacterEditorPage _editor = null!;
        private TestableAttackEditorPage _attackEditor = null!;

        /// <summary>
        ///     Seeds a player with one attack whose to-hit modifiers are <c>WIS+8</c> and opens that attack's editor.
        /// </summary>
        [SetUp]
        public async Task OpenAttackEditor()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                modifiers: ModifiersTemplateFactory.Build("WIS+8"),
                damages: DamageRollTemplateFactory.BuildCollection([DamageRollTemplateFactory.Build("1d6", DamageTypeEnum.Fire)])));
            _repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            _editor = await characterList.EditPlayer("Fighter");
            var attacks = await _editor.OpenAttacks();
            _attackEditor = await attacks.EditAttack("Longsword");
        }

        /// <summary>
        ///     A valid to-hit expression is stored once the attack and its character are saved.
        /// </summary>
        [Test]
        public async Task Should_Save_ToHit_Modifiers()
        {
            // Act
            await _attackEditor.MainInfo().SetToHitModifiers("STR+2");
            await CaptureAsync("before-save");
            var editor = await _attackEditor.Save();
            await editor.Save();

            // Assert
            StoredAttack(_repository).ToHitModifiers.Expression.Should().Be("STR+2");
        }

        /// <summary>
        ///     An invalid to-hit expression shows an error, blocks Save, and leaves the stored value unchanged.
        /// </summary>
        [Test]
        public async Task Should_Refuse_Invalid_ToHit_Modifiers()
        {
            // Act
            var mainInfo = _attackEditor.MainInfo();
            await mainInfo.SetToHitModifiers("abc");
            await _attackEditor.AttemptSave();
            await mainInfo.WaitForToHitModifiersError("Invalid dice throw modifiers template, should match something like MAS+STR+1.");
            await CaptureAsync("error-shown");
            await _attackEditor.ExpectStillOpen();
            await _attackEditor.Cancel();
            await _editor.Save();

            // Assert
            StoredAttack(_repository).ToHitModifiers.Expression.Should().Be("WIS+8");
        }

        /// <summary>
        ///     A dice term is not a valid to-hit modifier: it shows the error instead of reaching the model.
        /// </summary>
        [Test]
        public async Task Should_Refuse_A_Dice_Term_In_ToHit_Modifiers()
        {
            // Act
            var mainInfo = _attackEditor.MainInfo();
            await mainInfo.SetToHitModifiers("1d4");
            await _attackEditor.AttemptSave();
            await mainInfo.WaitForToHitModifiersError("Invalid dice throw modifiers template, should match something like MAS+STR+1.");
            await CaptureAsync("error-shown");
            await _attackEditor.ExpectStillOpen();
            await _attackEditor.Cancel();
            await _editor.Save();

            // Assert
            StoredAttack(_repository).ToHitModifiers.Expression.Should().Be("WIS+8");
        }

        /// <summary>
        ///     Canceling a valid to-hit edit leaves the stored value unchanged when the character is saved.
        /// </summary>
        [Test]
        public async Task Should_Discard_ToHit_Modifiers_On_Cancel()
        {
            // Act
            await _attackEditor.MainInfo().SetToHitModifiers("STR+2");
            await CaptureAsync("before-cancel");
            var editor = await _attackEditor.Cancel();
            await editor.Save();

            // Assert
            StoredAttack(_repository).ToHitModifiers.Expression.Should().Be("WIS+8");
        }
    }

    private static MartialAttackTemplate StoredAttack(ICharacterRepository repository)
    {
        return repository.GetAllCharacters().Single().MartialAttacks.Values.Single();
    }
}
