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
    [Route("[controller]")]
    [ApiController]
    public class ReleaseController : BasicController
    {
        public ReleaseController(ILogger<ReleaseController> logger, IWebHostEnvironment env, IConfiguration config) : base(logger, env, config)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReleases")]
        public async Task<ActionResult<List<List<Release>>>> GetReleases(ProjectRelease model)
        {
            try
            {
                //validate envrioment list.
                if(model.Environments == null || model.Environments.Count == 0)
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: environmentId is empty");
                    return BadRequest();
                }
                else
                {
                    List<List<Release>> result = new List<List<Release>>();

                    List<string> envrs = model.Environments;
                    //Get releases for each environment
                    foreach(var e in envrs)
                    {
                        List<Release> release = await this.GetReleaseById(model.ProjectId, e, model.NumberOfReleases);
                        result.Add(release);
                    }

                    return Ok(result);
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }
           
        }

        /// <summary>
        /// Get Release By ProjedtId
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReleasesByProjectId")]
        public async Task<ActionResult<List<Release>>> GetReleasesByProjectId([FromQuery] string projectId)
        {
            try
            {
                //Verify projectid
                if (!this.VerifyString(projectId))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: ProjectId is empty or null");
                    return BadRequest();
                }
                else
                {
                    //Filter releases
                    List<Release> releases = await this.FilterReleasesByProjectId(projectId);

                    //Sort release by created date from latest to oldest
                    releases.Sort((x, y) =>
                    {
                        return y.Created.CompareTo(x.Created);
                    });

                    return Ok(releases);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("CreateRelease")]
        public async Task<ActionResult<Release>> CreateNewRelease(Release model)
        {
            try
            {
                //Verify projectId
                if (!this.VerifyString(model.ProjectId))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: ProjectId is empty or null");
                    return BadRequest();
                }
                else
                {
                    //Validate release ID, if it is empty or null, generate  GUID 
                    if (string.IsNullOrEmpty(model.Id) || string.IsNullOrWhiteSpace(model.Id))
                        model.Id = Guid.NewGuid().ToString();

                    //Generate datetime
                    model.Created = DateTime.Now;

                    //Get current releases
                    List<Release> releases = await this.GetData<Release>("ReleaseFile");

                    //Add new release
                    releases.Add(model);

                    //Write back to the json file.
                    string json = JsonConvert.SerializeObject(releases);
                    string fileName = this.GetDataFile("ReleaseFile");
                    await System.IO.File.WriteAllTextAsync(fileName, json);

                    //Get release
                    Release r = await this.GetReleaseById(model.Id);

                    return Ok(r);
                }
            }catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }
          
        }
        
            /// <summary>
            /// 
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
        private async Task<bool> ValidateProjectRelease(string projectId, string environmentId)
        {
            //Validate projectId and EnvironmentId
            if (!this.VerifyString(projectId))
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: ProjectId is empty or null");
                return false;
            }
            else if (!this.VerifyString(environmentId))
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: Environment is empty or null");
                return false;
            }
            else
            {
                //Verify project
                var project = await this.GetProjectById(projectId);
                if (project == null)
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: failed to find project by {projectId}");
                    return false;
                }

                //Validate environment
                var env = await this.GetEnviromentById(environmentId);
                if (env == null)
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: failed to find envrionment by {environmentId}");
                    return false;
                }

                //this._logger.LogInformation($"{MethodBase.GetCurrentMethod().ReflectedType.Name} {projectId} {project.Name} EnviornmentId {environmentId} {env.Name}");
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private async Task<List<Release>> FilterReleasesByProjectId(string projectId)
        {
            List<Release> filteredReleases = new List<Release>();

            //VAlidate project id
            if(!this.VerifyString(projectId))
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: ProjectId is empty or null");
                return filteredReleases;
            }
            else
            {
                //Read Json file
                List<Release> releases = await this.GetData<Release>("ReleaseFile");

                //Filter
                filteredReleases = (from r in releases
                              where r.ProjectId == projectId
                              select r).ToList();

                return filteredReleases;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
      private async Task<List<Deployment>> GetDeployementsById(string releaseId, string environmentId)
        {
            try
            {
                List<Deployment> filteredDeployments = new List<Deployment>();

                //Validate releaseId and environmentId
                if(!this.VerifyString(releaseId))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: Failed to find release by {releaseId}");
                    return filteredDeployments;
                }
                else if (!this.VerifyString(environmentId))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: failed to find envrionment by {environmentId}");
                    return filteredDeployments;
                }
                else
                {
                    //Load deployments
                    List<Deployment> deployments = await this.GetData<Deployment>("DeploymentFile");

                    //Filter deployments
                    filteredDeployments  = (from d in deployments
                                        where d.ReleaseId == releaseId && d.EnvironmentId == environmentId
                                        select d).ToList();

                    return filteredDeployments;

                }
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Print logging
        /// </summary>
        /// <param name="releases"></param>
        private void PrintReleaseLogging(List<Release> releases, bool keep = false)
        {
            foreach(var r in releases)
            {
                if(r.Deployments != null)
                {
                    foreach (var d in r.Deployments)
                    {
                        if(!keep)
                            //Logging for all releases
                            this._logger.LogInformation($"| {r} | {d}");
                        else
                        {
                            //Logging for releases which are about to be kept
                            this._logger.LogInformation($"{r.Id} kept because it was the most recently deployed {d.EnvironmentId}");
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// Get Release By id
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="enviromentId"></param>
        /// <param name="numberOfRelease"></param>
        /// <returns></returns>
        private async Task<List<Release>> GetReleaseById(string projectId, string enviromentId, int numberOfRelease = 1)
        {
            try
            {
                //Validate projectId and environmentId
                bool isValid = await this.ValidateProjectRelease(projectId, enviromentId);

                if (!isValid)
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: projectId or envrionmentId is invalid");
                    return null;
                }
                else
                {
                    //Get releases by project id
                    List<Release> filteredRelease = await this.FilterReleasesByProjectId(projectId);
                    List<Release> result = new List<Release>();

                    //Get deployments by releaseid and environment id
                    if (filteredRelease.Count > 0)
                    {
                        //Get all deployments
                        for (int i = 0; i < filteredRelease.Count; i++)
                        {
                            Release r = filteredRelease[i];

                            //Get deployments
                            List<Deployment> deployments = await this.GetDeployementsById(r.Id, enviromentId);

                            //Verify whether the release has deployments
                            if (deployments.Count == 0)
                            {
                                //If the release does not have any deployments, it means that the release is unsuccessful, so remove the remove from the list
                                filteredRelease.RemoveAt(i);
                            }
                            else
                            {
                                r.Deployments = deployments;

                                //Sort Deployemnts by deployment date.
                                r.Deployments.Sort();
                            }

                        }

                        //Filter release by deployment date
                        if(filteredRelease.Count > 1)
                            filteredRelease.Sort();

                        //Logging 
                        this._logger.LogInformation($"There are {filteredRelease.Count} release for {projectId} project on {enviromentId} environment:");
                        
                        this.PrintReleaseLogging(filteredRelease);


                        //Return the number of release
                        if (numberOfRelease > filteredRelease.Count || numberOfRelease <= 0)
                        {
                            result = filteredRelease;
                        }
                        else
                        {
                            //Get n number of most recently deployed releases
                            result = filteredRelease.GetRange(0, numberOfRelease);
                        }
                        
                        //Logging
                        this.PrintReleaseLogging(result, true);

                        return result;

                    }
                    else
                    {
                        this._logger.LogWarning($"Warnning: {MethodBase.GetCurrentMethod().ReflectedType.Name}: not deployments are found for {projectId}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return null;
            }
        }

        
    }
}
