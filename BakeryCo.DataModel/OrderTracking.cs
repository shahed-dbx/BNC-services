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
    
    public partial class OrderTracking
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public Nullable<System.DateTime> TrackingTime { get; set; }
        public Nullable<int> AcceptedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string CancelReason { get; set; }
    }
}
