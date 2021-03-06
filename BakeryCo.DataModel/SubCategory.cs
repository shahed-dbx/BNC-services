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
    
    public partial class SubCategory
    {
        public SubCategory()
        {
            this.ItemsDetails = new HashSet<ItemsDetail>();
        }
    
        public int SubCategoryId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string ItemName_Ar { get; set; }
        public string Description_Ar { get; set; }
        public string AdditionalsId { get; set; }
        public Nullable<int> ModifierId { get; set; }
        public string Images { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> SubCatSeq { get; set; }
        public string BackgroundImage { get; set; }
        public string SectionIds { get; set; }
    
        public virtual MainCategory MainCategory { get; set; }
        public virtual ICollection<ItemsDetail> ItemsDetails { get; set; }
    }
}
