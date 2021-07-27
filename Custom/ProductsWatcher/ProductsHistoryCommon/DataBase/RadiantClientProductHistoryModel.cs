using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    public class RadiantClientProductHistoryModel : RadiantProductHistoryModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [ForeignKey("ProductDefinitionId")]
        public virtual RadiantClientProductDefinitionModel ProductDefinition { get; set; }

        public long ProductDefinitionId { get; set; }
    }
}
