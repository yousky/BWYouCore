using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public class Concurrency4MysqlStringModel : BWStringModel, IConcurrency4MysqlModel
    {
        [Timestamp]
        public virtual DateTime TimeStamp { get; set; }
    }
}
