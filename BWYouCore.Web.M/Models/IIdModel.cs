using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public interface IIdModel<TId> : IKeyModel
    {
        TId Id { get; set; }
    }
}
