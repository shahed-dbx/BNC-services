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
    
    public partial class DriverLocation
    {
        public int Id_DL { get; set; }
        public Nullable<int> DriverId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Nullable<System.DateTime> CurrentDT { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Rotation { get; set; }
    }
}
