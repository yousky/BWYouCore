using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public interface IConcurrency4MysqlModel
    {
        DateTime TimeStamp { get; set; }
    }
}
