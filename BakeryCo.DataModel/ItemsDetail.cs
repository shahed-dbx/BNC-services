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
    
    public partial class ItemsDetail
    {
        public ItemsDetail()
        {
            this.ItemPriceDetails = new HashSet<ItemPriceDetail>();
            this.OrderSubItems = new HashSet<OrderSubItem>();
            this.SubItemDetails = new HashSet<SubItemDetail>();
            this.SubItemDetails1 = new HashSet<SubItemDetail>();
        }
    
        public int ItemId { get; set; }
        public int SubCatId { get; set; }
        public Nullable<int> SectionId { get; set; }
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public string ItemName_Ar { get; set; }
        public string ItemDesc_Ar { get; set; }
        public Nullable<int> DisplayNameId { get; set; }
        public string images { get; set; }
        public Nullable<bool> availability { get; set; }
        public string ReMessageEn { get; set; }
        public string ReMessageAr { get; set; }
        public string ItemTypeIds { get; set; }
        public Nullable<int> Bindingtype { get; set; }
        public Nullable<int> AvgPreparationTime { get; set; }
        public Nullable<int> ItemSeq { get; set; }
        public Nullable<bool> IsDeliver { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string LayersShape { get; set; }
        public Nullable<bool> ReferanceImageOfCake { get; set; }
        public Nullable<bool> AllowImagesOnCake { get; set; }
    
        public virtual ICollection<ItemPriceDetail> ItemPriceDetails { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual ICollection<OrderSubItem> OrderSubItems { get; set; }
        public virtual ICollection<SubItemDetail> SubItemDetails { get; set; }
        public virtual ICollection<SubItemDetail> SubItemDetails1 { get; set; }
    }
}
