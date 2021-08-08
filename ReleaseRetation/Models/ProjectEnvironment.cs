using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseRetation.Models
{
    public class ProjectEnvironment : ItemBase
    {
        public ProjectEnvironment()
        {
        }

        public ProjectEnvironment(string id, string name) : base(id, name)
        {
        }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }
}
