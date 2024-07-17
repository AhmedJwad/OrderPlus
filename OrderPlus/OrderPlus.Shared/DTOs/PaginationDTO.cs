using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPlus.Shared.DTOs
{
    public class PaginationDTO
    {
        public int Id { get; set; }
        public int Page { get; set; } = 1;
        public int RecordsNumber { get; set; } = 10;
        public string? Filter { get; set; }

        public string? CategoryFilter { get; set; }
    }
}
