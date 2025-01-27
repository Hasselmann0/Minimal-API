using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Minimal_API.Dominio.Entidades
{
    public class Veiculo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(150)]
        public String Nome { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public String Marca { get; set; } = default!;
        [Required]
        [StringLength(10)]
        public int Ano { get; set; } = default!;
    }
}