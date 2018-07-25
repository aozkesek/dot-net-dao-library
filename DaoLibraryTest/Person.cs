using org.mao.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaoLibraryTest
{
    [Table("PERSONS", Schema = "TEST")]
    public class Person : AuditBaseModel
    {
        [Column("NAME")]
        public string Name { set; get; }

        [Column("SURNAME")]
        public string Surname { set; get; }

        public virtual IList<Phone> Phones { set; get; }
   
        public virtual IList<Address> Addresses { set; get; }

		public override string ToString()
		{
			return string.Format(
				"[Person: Id={2}, CreatedDateTime={3}, LastUpdateDateTime={4}, Name={0}, Surname={1}, LastUpdateCount={5}]"
				, Name, Surname, Id, CreateDateTime, LastUpdateDateTime, LastUpdateCount);
		}

    }
}
