using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///    A collection of <see cref="DamageRollTemplate"/>
/// </summary>
public class DamageRollTemplateCollection : List<DamageRollTemplate>, IHashable
{
}
