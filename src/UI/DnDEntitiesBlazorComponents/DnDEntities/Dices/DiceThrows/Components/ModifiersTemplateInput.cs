using System.Diagnostics.CodeAnalysis;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using NeoBlazorphic.Components.Inputs.Fields;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices.DiceThrows.Components;

public class ModifiersTemplateInput : NeoBaseInput<ModifiersTemplate>
{
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out ModifiersTemplate result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        validationErrorMessage = null;
        result = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            validationErrorMessage = "This expression cannot be empty";
            return false;
        }

        try
        {
            result = new ModifiersTemplate(value);
            return true;
        }
        catch (InvalidOperationException)
        {
            validationErrorMessage = "Invalid expression";
            return false;
        }
    }

    protected override string? FormatValueAsString(ModifiersTemplate? value)
    {
        return value?.Expression;
    }
}
