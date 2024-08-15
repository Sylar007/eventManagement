using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs
{
    public class AccountDetailsUpdateDto
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public Enums.Gender? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
