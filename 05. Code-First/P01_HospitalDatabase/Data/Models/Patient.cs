using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(80)]
        public string Email { get; set; }

        public bool HasInsurance { get; set; }

        public virtual ICollection<PatientMedicament> Prescriptions { get; set; } = new HashSet<PatientMedicament>();

        public virtual ICollection<Diagnose> Diagnoses { get; set; } = new HashSet<Diagnose>();

        public virtual ICollection<Visitation> Visitations { get; set; } = new HashSet<Visitation>();
    }
}
