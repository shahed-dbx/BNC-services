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
    
    public partial class SP_Get_PushNotificationMessages_Result
    {
        public int PID { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<long> OrderId { get; set; }
        public string DeviceToken { get; set; }
        public string PushMessage { get; set; }
        public Nullable<int> PushType { get; set; }
        public string PushNotificationType { get; set; }
        public Nullable<bool> isRead { get; set; }
        public string sentDate { get; set; }
    }
}
