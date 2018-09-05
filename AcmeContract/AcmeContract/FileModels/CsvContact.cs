using System;
using System.ComponentModel;

namespace AcmeContract.FileModels
{
    public class CsvContact
    {
        [DisplayName("first_name")]
        public string FirstName { get; set; }

        [DisplayName("last_name")]
        public string LastName { get; set; }

        [DisplayName("company_name")]
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Post { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string Web { get; set; }

    }
}