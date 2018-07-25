using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.mao.Model
{
    public class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID", Order = 1)]
        public long Id { set; get; }

        [Column("LAST_UPDATE_COUNT", Order = 2)]
        public int LastUpdateCount { set; get; }

		public override string ToString()
		{
			return string.Format("[Id={0}, LastUpdateCount={1}]", Id, LastUpdateCount);
		}
    }

    public class AuditBaseModel : BaseModel
    {
        [Column("LAST_UPDATE_DATETIME", Order = 3)]
        public DateTime LastUpdateDateTime { set; get; }

        [Column("CREATE_DATETIME", Order = 4)]
        public DateTime CreateDateTime { set; get; }

		public override string ToString()
		{
			return string.Format("[Id={0}, LastUpdateCount={1}, LastUpdateDateTime={2}, CreateDateTime={3}]"
			                     , Id, LastUpdateCount, LastUpdateDateTime, CreateDateTime);
		}

    }
}
