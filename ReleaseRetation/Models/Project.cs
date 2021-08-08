using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseRetation.Models
{
    public class Project : ItemBase
    {
        public Project()
        {
        }

        public Project(string id, string name) : base(id, name)
        {
        }
    }
}
