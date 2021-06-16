using System;

namespace Radiant.Common.Tasks.Triggers
{
    public abstract class RadiantTrigger
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string UID { get; set; } = Guid.NewGuid().ToString("D");
    }
}
