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
    
    public partial class SP_Get_DriverDetails_Result
    {
        public int Id_DL { get; set; }
        public Nullable<int> DriverId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<System.DateTime> CurrentDT { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int UserId { get; set; }
        public int CountryId { get; set; }
        public string FullName { get; set; }
        public string FamilyName { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
        public string Language { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> RandomNumber { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public Nullable<bool> IsBlocked { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<bool> IsDriver { get; set; }
        public Nullable<double> distance { get; set; }
    }
}