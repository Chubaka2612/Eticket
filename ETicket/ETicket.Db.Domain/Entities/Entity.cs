using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicket.Db.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual long Id { get; set; }

        [DataType(DataType.DateTime)]
        public virtual DateTime CreatedAt { get; set; }
    }
}
