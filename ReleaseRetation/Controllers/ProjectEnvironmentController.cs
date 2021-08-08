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
    public class ProjectEnvironmentController : BasicController
    {
        public ProjectEnvironmentController(ILogger<ReleaseController> logger, IWebHostEnvironment env, IConfiguration config) : base(logger, env, config)
        {
        }

        /// <summary>
        /// Get Environments
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Route("GetEnvironments")]
        public async Task<ActionResult<List<Project>>> GetProjectEnvironments()
        {
            try
            {
                List<ProjectEnvironment> projects = await this.GetData<ProjectEnvironment>("EnvironmentFile");
                return Ok(projects);
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Create new environment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateEnvironment")]
        public async Task<ActionResult<List<ProjectEnvironment>>> CreateNewEnvironment(ProjectEnvironment model)
        {
            try
            {
                //Verfiy project name
                if (string.IsNullOrEmpty(model.Name) || string.IsNullOrWhiteSpace(model.Name))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: project name is empty");
                    return BadRequest();
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Id) || string.IsNullOrWhiteSpace(model.Id))
                        model.Id = Guid.NewGuid().ToString();

                    //Write to json file
                    List<ProjectEnvironment> pEnvironments = await this.GetData<ProjectEnvironment>("EnvironmentFile");

                    //Add New Project
                    pEnvironments.Add(model);

                    //Write back to json file
                    string json = JsonConvert.SerializeObject(pEnvironments);
                    string fileName = this.GetDataFile("EnvironmentFile");
                    await System.IO.File.WriteAllTextAsync(fileName, json);

                    //Get new project and then return 
                   ProjectEnvironment environmentNew = await this.GetEnviromentById(model.Id);

                    return Ok(environmentNew);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();

            }

        }

    }
}
