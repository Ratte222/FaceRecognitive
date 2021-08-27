using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class FaceDetecrionDTO
    {
        public string ImageName { get; set; }
        public ICollection<int> Top { get; set; }
        public ICollection<int> Right { get; set; }
        public ICollection<int> Bottom { get; set; }
        public ICollection<int> Left { get; set; }
    }
}
