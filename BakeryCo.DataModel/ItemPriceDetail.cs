//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BakeryCo.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemPriceDetail
    {
        public int PriceId { get; set; }
        public int ItemId { get; set; }
        public int ItemSize { get; set; }
        public string ItemPrice { get; set; }
        public string ItemSizeDescription { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ItemSizeDescription_Ar { get; set; }
        public string CustomCakeJson { get; set; }
        public Nullable<decimal> Calories { get; set; }
        public string CaloriesUOM { get; set; }
    
        public virtual ItemSize ItemSize1 { get; set; }
        public virtual ItemsDetail ItemsDetail { get; set; }
    }
}
