﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public class Concurrency4MysqlIntModel : BWIntModel, IConcurrency4MysqlModel
    {
        [Timestamp]
        public virtual DateTime TimeStamp { get; set; }
    }
}
