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
    public class ProjectController : BasicController
    {
        public ProjectController(ILogger<ProjectController> logger, IWebHostEnvironment env, IConfiguration config) : base(logger, env, config)
        {
        }

        /// <summary>
        /// Get a list of projects
        /// </summary>
        /// <returns></returns>
        /// In reality, it should verify current session user's identity.
        [HttpGet]
        [Route("GetProjects")]
        public async Task<ActionResult<List<Project>>> GetAllProjects()
        {
            try
            {
                List<Project> projects = await this.GetData<Project>("ProjectFile");
                return Ok(projects);
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }

        }

        /// <summary>
        /// Create New Project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateProject")]
        public async Task<ActionResult<List<Project>>> CreateNewProject(Project model)
        {
            try
            {
                //Verfiy project name
                if (!this.VerifyString(model.Name))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: project name is empty");
                    return BadRequest();
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Id) || string.IsNullOrWhiteSpace(model.Id))
                        model.Id = Guid.NewGuid().ToString();

                    //Write to json file
                    List <Project> projects = await this.GetData<Project>("ProjectFile");

                    //Add New Project
                    projects.Add(model);

                    //Write back to json file
                    string json = JsonConvert.SerializeObject(projects);
                    string fileName = this.GetDataFile("ProjectFile");
                    await System.IO.File.WriteAllTextAsync(fileName, json);

                    //Get new project and then return 
                    Project newProject = await this.GetProjectById(model.Id);

                    return Ok(newProject);
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();

            }
            
        }

        /// <summary>
        /// Modify existing project name
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ModifyProject")]
        public async Task<ActionResult<Project>> ModifyProjectById(Project model)
        {
            try
            {
                //Verify project id
                if (!this.VerifyString(model.Id))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: projectId is empty");
                    return BadRequest();
                }
                else
                {
                    Project p = await this.GetProjectById(model.Id);

                    //Verify project exists or not
                    if (p == null)
                    {
                        this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: {model.Id} cannot be found");
                        return NotFound();
                    }
                    else
                    {
                        //Validate new project name
                        if(!string.IsNullOrEmpty(model.Name) && !string.IsNullOrWhiteSpace(model.Name) && p.Name != model.Name)
                            p.Name = model.Name;

                        //update its name
                        List<Project> projects = await this.GetData<Project>("ProjectFile");
                        foreach (var item in projects)
                        {
                            if (p.Id == item.Id) item.Name = p.Name;
                        }

                        //Write back to json file
                        string json = JsonConvert.SerializeObject(projects);
                        string fileName = this.GetDataFile("ProjectFile");
                        await System.IO.File.WriteAllTextAsync(fileName, json);

                        //Get updated project details
                        Project newProject = await this.GetProjectById(model.Id);

                        return Ok(newProject);
                    }
                }
            }catch(Exception ex)
            {
                this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}", ex.Message);
                return BadRequest();
            }
           
        }

        [HttpDelete]
        [Route("DeleteProject")]
        public async Task<ActionResult<List<Project>>> DeleteProjectById(Project model)
        {
            try
            {
                //Verfiy project name
                if (!this.VerifyString(model.Id))
                {
                    this._logger.LogError($"Error: {MethodBase.GetCurrentMethod().ReflectedType.Name}: projectId is empty");
                    return BadRequest();
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Id) || string.IsNullOrWhiteSpace(model.Id))
                        model.Id = Guid.NewGuid().ToString();

                    //Write to json file
                    List<Project> projects = await this.GetData<Project>("ProjectFile");
                   for(int i = 0; i < projects.Count; i++)
                    {
                        if (projects[i].Id == model.Id)
                            projects.RemoveAt(i);
                    }

                    //Write back to json file
                    string json = JsonConvert.SerializeObject(projects);
                    string fileName = this.GetDataFile("ProjectFile");
                    await System.IO.File.WriteAllTextAsync(fileName, json);

                    projects = await this.GetData<Project>("ProjectFile");

                    return Ok(projects);
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
