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
    
    public partial class OrderAssignDetail
    {
        public int OA_Id { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> StoreId { get; set; }
        public Nullable<int> DriverId { get; set; }
        public Nullable<System.DateTime> AssignTime { get; set; }
        public Nullable<System.DateTime> Order_AR { get; set; }
        public Nullable<bool> IsAccept { get; set; }
        public string Comment { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> IsDeletedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual StoreInformation StoreInformation { get; set; }
        public virtual OrderInfo OrderInfo { get; set; }
    }
}
