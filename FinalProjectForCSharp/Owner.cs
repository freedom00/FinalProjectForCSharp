using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FinalProjectForCSharp
{
    [Table("Owners")]
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [NotMapped]
        public int CarsNumber
        {
            get
            {
                try
                {
                    return CarsInGarage.Count();
                }
                catch (ArgumentNullException)
                {
                    return 0;
                }
            }
        }

        public byte[] Photo { get; set; }

        public ICollection<Car> CarsInGarage { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Id, Name);
        }
    }
}