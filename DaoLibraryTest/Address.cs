using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using org.mao.Model;

namespace DaoLibraryTest
{
    [Table("ADDRESSES", Schema = "TEST")]
    public class Address : BaseModel
    {
        [Column("COUNTRY")]
        public string Country { set; get; }

        [Column("CITY")]
        public string City { set; get; }

        [Column("COUNTY")]
        public string County { set; get; }

        [Column("POSTAL_CODE")]
        public string PostalCode { set; get; }

        [Column("LINE1")]
        public string Line1 { set; get; }

        [Column("LINE2")]
        public string Line2 { set; get; }

        [Column("PERSON_ID")]
		public long PersonId { set; get; }

		public virtual Person Person { set; get; }

        public override string ToString()
        {
            return string.Format("{4}\n{5}\n{3} {2} {1}\n{0}", Country, City, County, PostalCode, Line1, Line2);
        }
    }
}
