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
    
    public partial class CorporateClient
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string Msg { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> ReqApproved { get; set; }
        public string City { get; set; }
    }
}