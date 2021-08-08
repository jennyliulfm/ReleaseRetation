using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseRetation.Models;
using System.Reflection;
using Newtonsoft.Json;

namespace ReleaseRetation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeploymentController : BasicController
    {
        public DeploymentController(ILogger<BasicController> logger, IWebHostEnvironment env, IConfiguration config) : base(logger, env, config)
        {
        }

        [HttpGet]
        [Route("GetDeploymentsByEnvironmentId")]
        public async Task<ActionResult<List<Deployment>>> GetDeploymentsByEnvironmentId([FromQuery] string environmentId)
        {
            try
            {
                bool isValid = await this.ValidateEnvironmentId(environmentId);

                //Verify environmentId
                if (!isValid)
                { 
                    this._logger.LogWarning($"Warning: {MethodBase.GetCurrentMethod().ReflectedType.Name}: environmentId {environmentId}is empty or invalid");
                    return BadRequest();
                }
                else
                {
                    //Filter deployment
                    List<Deployment> deployments = await this.FilterDeploymentById(environmentId);

                    //Sort deployments from latest to oldest
                    deployments.Sort();

                    return Ok(deployments);
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
        [HttpPost]
        [Route("CreateDeployment")]
        public async Task<ActionResult<Deployment>> CreateDeployment(Deployment model)
        {
            try
            {
                bool isValid = await this.ValidateEnvironmentId(model.EnvironmentId);

                //Verify environmentId and releaseId
                if (!isValid)
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: environmentId {model.EnvironmentId} is empty or invalid");
                    return BadRequest();
                }
                else if(!this.VerifyString(model.ReleaseId))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: releaseId is empty or invalid");
                    return BadRequest();
                }
                else
                {
                    //Generate deploymentId
                    if (!this.VerifyString(model.Id))
                        model.Id = Guid.NewGuid().ToString();

                    //Set deployment date
                    model.DeployedAt = DateTime.Now;

                    //Get deployments
                    List<Deployment> deployments = await this.GetData<Deployment>("DeploymentFile");

                    //Add new deployment
                    deployments.Add(model);

                    //Write back to json file
                    string json = JsonConvert.SerializeObject(deployments);
                    string fileName = this.GetDataFile("DeploymentFile");
                    await System.IO.File.WriteAllTextAsync(fileName, json);

                    //Get new deployment
                    var deploymentNew = await this.GetDeploymentById(model.Id);

                    return Ok(deploymentNew);

                }

            }
            catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        private async Task<List<Deployment>> FilterDeploymentById(string environmentId)
        {
            try
            {
                //Load deployments
                var deployments = await this.GetData<Deployment>("DeploymentFile");

                //Filter deployments
                var filteredDeployments = (from d in deployments
                                           where d.EnvironmentId == environmentId
                                           select d).ToList();

                return filteredDeployments;
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return null;

            }
          
        }

        /// <summary>
        /// Validat EnvironmentId
        /// </summary>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        private async Task<bool> ValidateEnvironmentId(string environmentId)
        {
            if (!this.VerifyString(environmentId))
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: environmentId is empty or null");
                return false;
            }
            else
            {
                var env = await this.GetEnviromentById(environmentId);

                //Verify whethere environmentId is valid or not.
                if (env == null)
                {
                    this._logger.LogWarning($"Warning: {MethodBase.GetCurrentMethod().ReflectedType.Name}: Failed to find {environmentId} environment");
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }
    }
}
