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
    
    public partial class SP_Get_OrderDeliveryHistory_Result
    {
        public int orderId { get; set; }
        public Nullable<int> storeId { get; set; }
        public string StoreName { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Mobile { get; set; }
        public string DComments { get; set; }
        public string comments { get; set; }
        public Nullable<System.DateTime> orderDate { get; set; }
        public string orderType { get; set; }
        public string ExpectedTime { get; set; }
        public bool status { get; set; }
        public string OrderStatus { get; set; }
        public string Total_Price { get; set; }
        public string PaymentMode { get; set; }
        public string InvoiceNo { get; set; }
    }
}