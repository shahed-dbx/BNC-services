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
    
    public partial class OrderInfo
    {
        public OrderInfo()
        {
            this.OrderAssignDetails = new HashSet<OrderAssignDetail>();
            this.OrderItems = new HashSet<OrderItem>();
            this.OrderItems1 = new HashSet<OrderItem>();
        }
    
        public int OrderId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string Comments { get; set; }
        public string AppVersion { get; set; }
        public string merchantTransactionId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string OrderType { get; set; }
        public string ExpectedTime { get; set; }
        public string OrderStatus { get; set; }
        public Nullable<decimal> Total_Price { get; set; }
        public Nullable<decimal> DeliveryCharges { get; set; }
        public Nullable<decimal> VatCharges { get; set; }
        public Nullable<decimal> VatPercentage { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> NetTotal { get; set; }
        public string Device_token { get; set; }
        public bool status { get; set; }
        public Nullable<bool> IsFavorite { get; set; }
        public string FavoriteName { get; set; }
        public Nullable<int> PaymentMode { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> AddressID { get; set; }
        public Nullable<int> PreparationTime { get; set; }
        public Nullable<int> TravelTime { get; set; }
        public Nullable<System.DateTime> KichenStartTime { get; set; }
        public Nullable<int> PrayerTime { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<long> IsDeletedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual ICollection<OrderAssignDetail> OrderAssignDetails { get; set; }
        public virtual Registration Registration { get; set; }
        public virtual UserAddressDetail UserAddressDetail { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems1 { get; set; }
    }
}