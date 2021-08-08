using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReleaseRetation.Models
{
    public class Release : IComparable<Release>
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        public DateTime Created { get; set; }

        public List<Deployment> Deployments { get; set; }

        //Sort release by deployment date from latest to oldest
        public int CompareTo(Release other)
        {
            if (this.Deployments != null && other.Deployments != null && this.Deployments.Count > 0 && other.Deployments.Count > 0)
            {
                return other.Deployments[0].DeployedAt.CompareTo(this.Deployments[0].DeployedAt);
            }
            else
                return -1;
        }

        public override string ToString()
        {
            return $"{this.Id} (Version: {this.Version}, Created: {this.Created})";
        }

        public Release()
        {
            this.Deployments = new List<Deployment>();
        }

        public Release(string id, string projectId, string version, DateTime created)
        {
            Id = id;
            ProjectId = projectId;
            Version = version;
            Created = created;
            this.Deployments = new List<Deployment>();
        }
    }
}
