using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using org.mao.Model;

namespace DaoLibraryTest
{
    [Table("PHONES", Schema = "TEST")]
    public class Phone : BaseModel
    {

        [Column("COUNTRY_CODE")]
        public string CountryCode { set; get; }

        [Column("AREA_CODE")]
        public string AreaCode { set; get; }

        [Column("PHONE_NUMBER")]
        public string PhoneNumber { set; get; }

		[Column("PERSON_ID")]
		public long PersonId { set; get; }

		public virtual Person Person { set; get; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", CountryCode, AreaCode, PhoneNumber);
        }
    
    }
}
