using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ReleaseRetation.Models
{
    public class ProjectRelease
    {

        public string ProjectId { get; set; }

        public string EnvironmentId { get; set; }

        [JsonProperty("Environments")]
        public List<string> Environments { get; set; }
        
        [System.ComponentModel.DefaultValue(1)]
        public int NumberOfReleases { get; set; }

        public ProjectRelease()
        {
            this.Environments = new List<string>();
        }

        public ProjectRelease(string projectId, string environmentId, int numberOfReleases = 1)
        {
            ProjectId = projectId;
            EnvironmentId = environmentId;
            NumberOfReleases = numberOfReleases;

            this.Environments = new List<string>();
            this.Environments.Add(environmentId);
        }
    }
}
