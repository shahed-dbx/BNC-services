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
    
    public partial class OrderItem
    {
        public OrderItem()
        {
            this.OrderSubItems = new HashSet<OrderSubItem>();
        }
    
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public string Comments { get; set; }
        public string ItemPrice { get; set; }
        public Nullable<int> Size { get; set; }
        public Nullable<int> ItemType { get; set; }
    
        public virtual ICollection<OrderSubItem> OrderSubItems { get; set; }
        public virtual OrderInfo OrderInfo { get; set; }
        public virtual OrderInfo OrderInfo1 { get; set; }
    }
}