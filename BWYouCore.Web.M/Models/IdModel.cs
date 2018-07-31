using BWYouCore.Web.M.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BWYouCore.Web.M.Models
{
    public class IdModel<TId> : IIdModel<TId>
    {
        [Key]
        [Display(Name = "ID")]
        [Filterable(false)]
        public virtual TId Id { get; set; }

        string IDbModel.ToString()
        {
            return Id.ToString();
        }

        public string GetKeyName()
        {
            return "Id";
        }
    }
}
