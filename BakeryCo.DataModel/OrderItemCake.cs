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
    
    public partial class OrderItemCake
    {
        public int Id { get; set; }
        public Nullable<int> OrderItemId { get; set; }
        public Nullable<long> OrderId { get; set; }
        public string MessageOnCake { get; set; }
        public string RefImage { get; set; }
        public string ImsgrOnCake { get; set; }
        public string Shape { get; set; }
        public Nullable<int> Layers { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
}
