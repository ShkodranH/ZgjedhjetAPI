using Shkodran_Hasani_Zgjedhjet_API.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shkodran_Hasani_Zgjedhjet_API.Models.Entities
{
    public class Zgjedhjet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Kategoria Kategoria { get; set; }
        
        [Required]
        public Komuna Komuna { get; set; }

        [Required]
        [MaxLength(100)]
        public string QendraVotimit { get; set; }

        [Required]
        [MaxLength(100)]
        public string VendVotimi { get; set; }

        [Required]
        public Partia Partia { get; set; }

        [Required]
        public int Vota { get; set; }

    }
}
