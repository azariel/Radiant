using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    public class RadiantClientProductDefinitionModel : RadiantProductDefinitionModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [ForeignKey("ProductId")]
        public virtual RadiantClientProductModel Product { get; set; }

        public long ProductId { get; set; }

        public virtual List<RadiantClientProductHistoryModel> ProductHistoryCollection { get; set; } = new();
    }
}
