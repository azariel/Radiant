using System.Collections.Generic;
using Radiant.Common.Tasks;

namespace Radiant.Common.Configuration
{
    /// <summary>
    /// Configuration for tasks in Radiant
    /// </summary>
    public class RadiantTasksConfig
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<IRadiantTask> Tasks { get; set; } = new();
    }
}
