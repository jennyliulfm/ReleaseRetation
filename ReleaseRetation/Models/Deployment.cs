using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseRetation.Models
{
    public class Deployment: IComparable<Deployment>
    {
        public string Id { get; set; }
        public string ReleaseId { get; set; }

        public string EnvironmentId { get; set; }

        public DateTime DeployedAt { get; set; }

        public int CompareTo(Deployment other)
        {
            return other.DeployedAt.CompareTo(this.DeployedAt);
                
        }

        /// <summary>
        /// Sortin deployments by Deploymed At from latest to oldest
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Id} (DeployedAt: {this.DeployedAt})";
        }

        public Deployment()
        {
        }

        public Deployment(string id, string releaseId, string environmentId, DateTime deployedAt)
        {
            Id = id;
            ReleaseId = releaseId;
            EnvironmentId = environmentId;
            DeployedAt = deployedAt;
        }
    }
}
