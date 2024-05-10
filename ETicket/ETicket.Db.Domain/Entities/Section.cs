using System.Collections.Generic;

namespace ETicket.Db.Domain.Entities
{
    public class Section : Entity
    {
        public long ManifestId { get; set; }

        public string Name { get; set; }

        public Manifest Manifest { get; set; }

        public ICollection<Row> Rows { get; set; }

    }
}
