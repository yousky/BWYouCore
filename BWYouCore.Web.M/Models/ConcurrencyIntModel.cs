using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public class ConcurrencyIntModel : BWIntModel, IConcurrencyModel
    {
        [Timestamp]
        public virtual byte[] RowVersion { get; set; }
    }
}
