using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.UiTestNavigation.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.UiTestNavigation.Scenarios;

/// <summary>
///     ATTACK_CRUD: from a character in the editor, add, duplicate, edit and delete attacks through the attack editor
///     page, asserting the attack list reflects each operation — all through typed objects.
/// </summary>
public sealed class AttackCrudScenarios : IsolatedScenarioFixture
{
    /// <summary>
    ///     Adds, duplicates, edits and deletes attacks on a seeded character through the UI.
    /// </summary>
    [Test]
    public async Task Should_Add_Duplicate_Edit_And_Delete_Attacks()
    {
        // Arrange
        var repository = Services.GetRequiredService<ICharacterRepository>();
        repository.Save(CharacterFactory.BuildPlayer(name: "Fighter"));

        var list = new TestCharacterListEditorPage(Page, BaseUrl);
        await list.GoToAsync();
        var editor = await list.EditPlayer("Fighter");

        // Act + Assert: add.
        var attacks = await editor.OpenAttacks();
        var attackEditor = await attacks.AddAttack();
        await attackEditor.MainInfo().SetName("Longsword");
        await attackEditor.Save();
        attacks = await editor.OpenAttacks();
        await CaptureAsync("attack-added");
        (await attacks.AttackNames()).Should().Contain("Longsword");

        // Act + Assert: duplicate.
        var duplicateEditor = await attacks.DuplicateAttack("Longsword");
        await duplicateEditor.Save();
        attacks = await editor.OpenAttacks();
        (await attacks.AttackNames()).Count(name => name == "Longsword").Should().Be(2);

        // Act + Assert: edit one copy's name.
        var editAttackEditor = await attacks.EditAttack("Longsword");
        await editAttackEditor.MainInfo().SetName("Greatsword");
        await editAttackEditor.Save();
        attacks = await editor.OpenAttacks();
        (await attacks.AttackNames()).Should().Contain("Greatsword");

        // Act + Assert: delete it.
        await attacks.DeleteAttack("Greatsword");
        await CaptureAsync("attack-deleted");
        (await attacks.AttackNames()).Should().NotContain("Greatsword");
    }
}
