using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Radiant.Custom.ProductsHistoryCommon.DataBase
{
    public class RadiantServerProductDefinitionModel : RadiantProductDefinitionModel
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [ForeignKey("ProductId")]
        public virtual RadiantServerProductModel Product { get; set; }

        public long ProductId { get; set; }
        public virtual List<RadiantServerProductHistoryModel> ProductHistoryCollection { get; set; } = new();
    }
}
