using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string UserName { get; set; }
        [Column("chat_id")]
        public long ChatId { get; set; }
        [Column("is_operator")]
        public bool IsOperator { get; set; }
        [Column("avatar")]
        public string Avatar {  get; set; }
    }
}
