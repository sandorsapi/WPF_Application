using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    [Table("ATTS")]
    public class DataTable
    {
        public DataTable() {}

        [Key]
        public long ID { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public long Value { get; set; }

        [Required]
        public string Symbol { get; set; }
    }
}