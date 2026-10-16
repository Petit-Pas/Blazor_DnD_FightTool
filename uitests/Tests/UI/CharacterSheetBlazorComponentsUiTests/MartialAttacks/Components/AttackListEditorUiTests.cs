using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DomainTestsUtilities.Factories.Characters;
using DomainTestsUtilities.Factories.MartialAttacks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.MartialAttacks.Components;

/// <summary>
///     Independent creation, editing and deletion capabilities of the attack list.
/// </summary>
public static class AttackListEditorUiTests
{
    /// <summary>
    ///     Rendering attack data seeded through the repository.
    /// </summary>
    public sealed class DisplayAttacks : IsolatedScenarioFixture
    {
        /// <summary>
        ///     The attack list renders the character's stored attacks.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Attacks()
        {
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"),
                MartialAttackTemplateFactory.Build(name: "Bow"));

            var attacks = await editor.OpenAttacks();

            (await attacks.AttackNames()).Should().BeEquivalentTo("Longsword", "Bow");
        }

        /// <summary>
        ///     The attack editor renders the name supplied by the repository.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Attack_In_Editor()
        {
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"));
            var attacks = await editor.OpenAttacks();

            var attackEditor = await attacks.EditAttack("Longsword");

            (await attackEditor.MainInfo().GetName()).Should().Be("Longsword");
        }
    }

    /// <summary>
    ///     Creating attacks from scratch or by duplication, including cancellation.
    /// </summary>
    public sealed class CreateAttacks : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Saving a new attack adds it to the character's attack list.
        /// </summary>
        [Test]
        public async Task Should_Create_An_Attack()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services);
            var attacks = await editor.OpenAttacks();

            // Act
            var attackEditor = await attacks.AddAttack();
            await attackEditor.MainInfo().SetName("Longsword");
            await attackEditor.Save();
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().Equal("Longsword");
        }

        /// <summary>
        ///     Saving a duplicate creates a second attack while preserving the original and other attacks.
        /// </summary>
        [Test]
        public async Task Should_Create_An_Attack_By_Duplication()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"),
                MartialAttackTemplateFactory.Build(name: "Bow"));
            var attacks = await editor.OpenAttacks();

            // Act
            var duplicateEditor = await attacks.DuplicateAttack("Longsword");
            await duplicateEditor.Save();
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().BeEquivalentTo("Longsword", "Longsword", "Bow");
        }

        /// <summary>
        ///     Canceling a new attack leaves the existing attack list unchanged.
        /// </summary>
        [Test]
        public async Task Should_Discard_A_New_Attack_On_Cancel()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"));
            var attacks = await editor.OpenAttacks();

            // Act
            var attackEditor = await attacks.AddAttack();
            await attackEditor.MainInfo().SetName("Discarded");
            await attackEditor.Cancel();
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().Equal("Longsword");
        }

        /// <summary>
        ///     Canceling a duplicate leaves the existing attack list unchanged.
        /// </summary>
        [Test]
        public async Task Should_Discard_A_Duplicate_On_Cancel()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"));
            var attacks = await editor.OpenAttacks();

            // Act
            var attackEditor = await attacks.DuplicateAttack("Longsword");
            await attackEditor.MainInfo().SetName("Discarded");
            await attackEditor.Cancel();
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().Equal("Longsword");
        }
    }

    /// <summary>
    ///     Saving and canceling changes to an existing attack.
    /// </summary>
    public sealed class EditAttacks : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Editing an attack updates only that entry without adding a copy.
        /// </summary>
        [Test]
        public async Task Should_Save_Changes_To_An_Attack()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"),
                MartialAttackTemplateFactory.Build(name: "Bow"));
            var attacks = await editor.OpenAttacks();

            // Act
            var editAttackEditor = await attacks.EditAttack("Longsword");
            await editAttackEditor.MainInfo().SetName("Greatsword");
            await editAttackEditor.Save();
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().BeEquivalentTo("Greatsword", "Bow");
        }

        /// <summary>
        ///     Canceling an attack edit preserves its name when the parent character is saved.
        /// </summary>
        [Test]
        public async Task Should_Discard_Changes_On_Cancel()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Longsword"));
            var attacks = await editor.OpenAttacks();
            var attackEditor = await attacks.EditAttack("Longsword");

            // Act
            await attackEditor.MainInfo().SetName("Discarded");
            await attackEditor.Cancel();
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().Equal("Longsword");
        }
    }

    /// <summary>
    ///     Deleting the selected attack row without changing other displayed entries.
    /// </summary>
    public sealed class DeleteAttacks : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Clicking Delete removes only the selected attack from the saved character.
        /// </summary>
        [Test]
        public async Task Should_Delete_Only_The_Selected_Attack()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services,
                MartialAttackTemplateFactory.Build(name: "Target"),
                MartialAttackTemplateFactory.Build(name: "Bow"));
            var attacks = await editor.OpenAttacks();

            // Act
            await attacks.DeleteAttack("Target");
            await editor.Save();

            // Assert
            StoredAttackNames(Services).Should().Equal("Bow");
        }
    }

    private static string[] StoredAttackNames(IServiceProvider services)
    {
        return services.GetRequiredService<ICharacterRepository>().GetAllCharacters().Single()
            .MartialAttacks.Values.Select(attack => attack.Name).ToArray();
    }

    private static async Task<TestableCharacterEditorPage> OpenCharacter(
        IPage page, string baseUrl, IServiceProvider services, params MartialAttackTemplate[] attacks)
    {
        var character = CharacterFactory.BuildPlayer(name: "Fighter");
        foreach (var attack in attacks)
        {
            character.MartialAttacks.Add(attack.Id, attack);
        }

        services.GetRequiredService<ICharacterRepository>().Save(character);
        var list = new TestableCharacterListEditorPage(page, baseUrl);
        await list.GoToAsync();
        return await list.EditPlayer("Fighter");
    }
}
