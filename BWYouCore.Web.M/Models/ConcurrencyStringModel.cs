using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public class ConcurrencyStringModel : BWStringModel, IConcurrencyModel
    {
        [Timestamp]
        public virtual byte[] RowVersion { get; set; }
    }
}
