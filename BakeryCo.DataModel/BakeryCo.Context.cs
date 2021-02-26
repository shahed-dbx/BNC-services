﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BakeryCoEntities : DbContext
    {
        public BakeryCoEntities()
            : base("name=BakeryCoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Registration> Registrations { get; set; }
        public virtual DbSet<MainCategory> MainCategories { get; set; }
        public virtual DbSet<OrderTracking> OrderTrackings { get; set; }
        public virtual DbSet<PaymentMode> PaymentModes { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<OrderAssignDetail> OrderAssignDetails { get; set; }
        public virtual DbSet<PushNotificationCustomer> PushNotificationCustomers { get; set; }
        public virtual DbSet<DriverLocation> DriverLocations { get; set; }
        public virtual DbSet<SMSDetail> SMSDetails { get; set; }
        public virtual DbSet<CorporateClient> CorporateClients { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<UserRating> UserRatings { get; set; }
        public virtual DbSet<UserPromoDetail> UserPromoDetails { get; set; }
        public virtual DbSet<OrderSubItem> OrderSubItems { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<SubItemDetail> SubItemDetails { get; set; }
        public virtual DbSet<StoreWorkData> StoreWorkDatas { get; set; }
        public virtual DbSet<ItemPriceDetail> ItemPriceDetails { get; set; }
        public virtual DbSet<userPromotion> userPromotions { get; set; }
        public virtual DbSet<VATDetail> VATDetails { get; set; }
        public virtual DbSet<StoreInformation> StoreInformations { get; set; }
        public virtual DbSet<ItemSize> ItemSizes { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<ItemsDetail> ItemsDetails { get; set; }
        public virtual DbSet<OrderRating> OrderRatings { get; set; }
        public virtual DbSet<UserAddressDetail> UserAddressDetails { get; set; }
        public virtual DbSet<OrderInfo> OrderInfoes { get; set; }
        public virtual DbSet<UserDeviceToken> UserDeviceTokens { get; set; }
        public virtual DbSet<OrderItemCake> OrderItemCakes { get; set; }
        public virtual DbSet<OrderItemCakeLayer> OrderItemCakeLayers { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
    
        public virtual ObjectResult<string> SP_CancelOrder(Nullable<int> orderId, Nullable<int> userId)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("orderId", orderId) :
                new ObjectParameter("orderId", typeof(int));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_CancelOrder", orderIdParameter, userIdParameter);
        }
    
        public virtual ObjectResult<SP_Get_DriverDetails_Result> SP_Get_DriverDetails(Nullable<int> storeId)
        {
            var storeIdParameter = storeId.HasValue ?
                new ObjectParameter("StoreId", storeId) :
                new ObjectParameter("StoreId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_DriverDetails_Result>("SP_Get_DriverDetails", storeIdParameter);
        }
    
        public virtual ObjectResult<SP_Get_OrderDeliveryHistory_Result> SP_Get_OrderDeliveryHistory(Nullable<int> driverId)
        {
            var driverIdParameter = driverId.HasValue ?
                new ObjectParameter("DriverId", driverId) :
                new ObjectParameter("DriverId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_OrderDeliveryHistory_Result>("SP_Get_OrderDeliveryHistory", driverIdParameter);
        }
    
        public virtual ObjectResult<SP_Get_Promotions_Result> SP_Get_Promotions(Nullable<int> userid, string deviceToken)
        {
            var useridParameter = userid.HasValue ?
                new ObjectParameter("Userid", userid) :
                new ObjectParameter("Userid", typeof(int));
    
            var deviceTokenParameter = deviceToken != null ?
                new ObjectParameter("DeviceToken", deviceToken) :
                new ObjectParameter("DeviceToken", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_Promotions_Result>("SP_Get_Promotions", useridParameter, deviceTokenParameter);
        }
    
        public virtual ObjectResult<procSelectStoreInfo_Result> procSelectStoreInfo(Nullable<int> storeId)
        {
            var storeIdParameter = storeId.HasValue ?
                new ObjectParameter("storeId", storeId) :
                new ObjectParameter("storeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<procSelectStoreInfo_Result>("procSelectStoreInfo", storeIdParameter);
        }
    
        public virtual ObjectResult<SP_Get_ItemDetailsNew_Result> SP_Get_ItemDetailsNew()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_ItemDetailsNew_Result>("SP_Get_ItemDetailsNew");
        }
    
        public virtual ObjectResult<string> SP_Get_ItemDetailsNewTest()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_Get_ItemDetailsNewTest");
        }
    
        public virtual ObjectResult<GenerateNewOrderPN_Result> GenerateNewOrderPN(Nullable<long> orderId, Nullable<int> userId, Nullable<int> flag)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(long));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var flagParameter = flag.HasValue ?
                new ObjectParameter("Flag", flag) :
                new ObjectParameter("Flag", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GenerateNewOrderPN_Result>("GenerateNewOrderPN", orderIdParameter, userIdParameter, flagParameter);
        }
    
        public virtual ObjectResult<string> SP_Delete_PushNotificationMsgCustomer(Nullable<int> pID, Nullable<int> flag)
        {
            var pIDParameter = pID.HasValue ?
                new ObjectParameter("PID", pID) :
                new ObjectParameter("PID", typeof(int));
    
            var flagParameter = flag.HasValue ?
                new ObjectParameter("Flag", flag) :
                new ObjectParameter("Flag", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_Delete_PushNotificationMsgCustomer", pIDParameter, flagParameter);
        }
    
        public virtual ObjectResult<SP_Get_PushNotificationMessages_Result> SP_Get_PushNotificationMessages(Nullable<int> userId, Nullable<int> userType)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var userTypeParameter = userType.HasValue ?
                new ObjectParameter("UserType", userType) :
                new ObjectParameter("UserType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_PushNotificationMessages_Result>("SP_Get_PushNotificationMessages", userIdParameter, userTypeParameter);
        }
    
        public virtual ObjectResult<string> Save_OrderRating(Nullable<int> orderId, Nullable<decimal> orderRating, string orderComment, Nullable<decimal> driverRating, string driverComment)
        {
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            var orderRatingParameter = orderRating.HasValue ?
                new ObjectParameter("OrderRating", orderRating) :
                new ObjectParameter("OrderRating", typeof(decimal));
    
            var orderCommentParameter = orderComment != null ?
                new ObjectParameter("OrderComment", orderComment) :
                new ObjectParameter("OrderComment", typeof(string));
    
            var driverRatingParameter = driverRating.HasValue ?
                new ObjectParameter("DriverRating", driverRating) :
                new ObjectParameter("DriverRating", typeof(decimal));
    
            var driverCommentParameter = driverComment != null ?
                new ObjectParameter("DriverComment", driverComment) :
                new ObjectParameter("DriverComment", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("Save_OrderRating", orderIdParameter, orderRatingParameter, orderCommentParameter, driverRatingParameter, driverCommentParameter);
        }
    
        public virtual int SP_Get_TrackOrder(Nullable<int> userId, Nullable<int> orderId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_Get_TrackOrder", userIdParameter, orderIdParameter);
        }
    
        public virtual ObjectResult<procSelectSpecifiedDayStoreTime_Result> procSelectSpecifiedDayStoreTime(string dayOfWeek, string userVersion, string appType)
        {
            var dayOfWeekParameter = dayOfWeek != null ?
                new ObjectParameter("dayOfWeek", dayOfWeek) :
                new ObjectParameter("dayOfWeek", typeof(string));
    
            var userVersionParameter = userVersion != null ?
                new ObjectParameter("UserVersion", userVersion) :
                new ObjectParameter("UserVersion", typeof(string));
    
            var appTypeParameter = appType != null ?
                new ObjectParameter("AppType", appType) :
                new ObjectParameter("AppType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<procSelectSpecifiedDayStoreTime_Result>("procSelectSpecifiedDayStoreTime", dayOfWeekParameter, userVersionParameter, appTypeParameter);
        }
    
        public virtual ObjectResult<SP_Get_OrderTracking_New_Result> SP_Get_OrderTracking_New(Nullable<int> userId, Nullable<int> orderId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_OrderTracking_New_Result>("SP_Get_OrderTracking_New", userIdParameter, orderIdParameter);
        }
    
        public virtual ObjectResult<SP_Get_AssignOrderDetails_Result> SP_Get_AssignOrderDetails(Nullable<int> driverId)
        {
            var driverIdParameter = driverId.HasValue ?
                new ObjectParameter("DriverId", driverId) :
                new ObjectParameter("DriverId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_AssignOrderDetails_Result>("SP_Get_AssignOrderDetails", driverIdParameter);
        }
    
        public virtual ObjectResult<SP_Get_OrderTracking_Result> SP_Get_OrderTracking(Nullable<int> userId, Nullable<int> orderId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var orderIdParameter = orderId.HasValue ?
                new ObjectParameter("OrderId", orderId) :
                new ObjectParameter("OrderId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_Get_OrderTracking_Result>("SP_Get_OrderTracking", userIdParameter, orderIdParameter);
        }
    }
}
