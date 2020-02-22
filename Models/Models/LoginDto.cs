using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class LoginDto
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public int VerifyCode { get; set; }
    }
}
