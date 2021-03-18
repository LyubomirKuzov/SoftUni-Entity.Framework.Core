using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstates.Models
{
    public class PropertyTag
    {
        //If .NET Core is not available use mapping table

        public int PropertyId { get; set; }

        public virtual Property Property { get; set; }

        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
