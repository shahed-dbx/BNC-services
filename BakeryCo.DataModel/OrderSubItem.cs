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
    
    public partial class OrderSubItem
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int OrderSubItemId { get; set; }
        public int Qty { get; set; }
        public string Comments { get; set; }
        public string ItemPrice { get; set; }
        public Nullable<int> Size { get; set; }
    
        public virtual ItemsDetail ItemsDetail { get; set; }
        public virtual OrderItem OrderItem { get; set; }
    }
}
