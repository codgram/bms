using System.ComponentModel;

namespace Application.Model.Enum;

public enum WorkHeaderType
{
    Picking,
    Receiving,
    Loading,
    Move,

    [Description("Cycle Count")]
    CycleCount,
}