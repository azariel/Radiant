using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsWatcher.ProductsHistoryCommon.DataBase
{
    public class RadiantServerProductHistoryModel : RadiantProductHistoryModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [ForeignKey("ProductDefinitionId")]
        public virtual RadiantServerProductDefinitionModel ProductDefinition { get; set; }

        public long ProductDefinitionId { get; set; }
    }
}
