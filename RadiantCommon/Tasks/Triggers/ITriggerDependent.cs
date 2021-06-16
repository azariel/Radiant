using System.Collections.Generic;

namespace Radiant.Common.Tasks.Triggers
{
    public interface ITriggerDependent
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        List<ITrigger> Triggers { get; set; }
    }
}
