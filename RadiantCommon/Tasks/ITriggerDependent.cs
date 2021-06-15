using System.Collections.Generic;
using Radiant.Common.Tasks.Triggers;

namespace Radiant.Common.Tasks
{
    public interface ITriggerDependent
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<ITrigger> Triggers { get; set; }
    }
}
