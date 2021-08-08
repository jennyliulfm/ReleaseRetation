using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseRetation.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ReleaseRetation.Controllers
{

    /// <summary>
    /// Define a base controller to manage common functions between different controllers
    /// </summary>
    public class BasicController : ControllerBase
    {
        public readonly ILogger<BasicController> _logger;
        public readonly IWebHostEnvironment _env;
        public readonly IConfiguration _config;

        public BasicController(ILogger<BasicController> logger, IWebHostEnvironment env, IConfiguration config)
        {
            this._logger = logger;
            this._env = env;
            this._config = config;
        }

        /// <summary>
        /// Get project by Id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<Project> GetProjectById(string projectId)
        {
            List<Project> projects = await this.GetData<Project>("ProjectFile");

            foreach (var p in projects)
            {
                if (p.Id == projectId) return p;
            }

            return null;
        }

        /// <summary>
        /// Get Project Environment by Id
        /// </summary>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<ProjectEnvironment> GetEnviromentById(string environmentId)
        {
            List<ProjectEnvironment> envs = await this.GetData<ProjectEnvironment>("EnvironmentFile");
            foreach(var e in envs)
            {
                if (e.Id == environmentId) return e;
            }

            return null;
        }

        /// <summary>
        /// Get Release By Id
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<Release> GetReleaseById(string releaseId)
        {
            List<Release> releases = await this.GetData<Release>("ReleaseFile");
            foreach (var r in releases)
            {
                if (r.Id == releaseId) return r;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<Deployment> GetDeploymentById(string deploymentId)
        {
            List<Deployment> deployments = await this.GetData<Deployment>("DeploymentFile");
            foreach (var d in deployments)
            {
                if (d.Id == deploymentId) return d;
            }

            return null;
        }



        /// <summary>
        /// Load Json data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<T>> GetData<T>(string file)
        {
            try
            {
                string filePath = this.GetDataFile(file);

                if (System.IO.File.Exists(filePath))
                {
                    string json = await System.IO.File.ReadAllTextAsync(filePath);
                    List<T> data = JsonConvert.DeserializeObject<List<T>>(json);

                    return data;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
  
                return null;
            }
        }

        /// <summary>
        /// Get each Json file name
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [NonAction]
        public string GetDataFile(string file)
        {
            string fileName = this._config.GetValue<string>($"DataFiles:{file}");

            string filePath = this._env.ContentRootPath
                          + Path.DirectorySeparatorChar.ToString()
                          + fileName;
           
            return filePath;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        
        [NonAction]
        public bool VerifyString(string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
