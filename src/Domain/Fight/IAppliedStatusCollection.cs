﻿using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

namespace DnDFightTool.Domain.Fight;

public interface IAppliedStatusCollection
{
    void Add(AppliedStatus appliedStatus);

    public event AppliedStatusUpdatedEventHandler AppliedStatusUpdated;

    IEnumerable<AppliedStatus> GetStatusAppliedTo(Guid affectedCharacterId);
}