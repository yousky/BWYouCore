using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public interface IConcurrencyModel
    {
        byte[] RowVersion { get; set; }
    }
}
