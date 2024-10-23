using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    [Table("operators")]
    public class Operator
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string UserName { get; set; }

    }
}
