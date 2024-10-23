using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    [Table("hashes")]
    public class Hash
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("token")]
        public string Token { get; set; }
        [Column("salt")]
        public string Salt { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("operator_id")]
        public int OperatorId { get; set; }

        public Operator Operator { get; set; }
    }
}
