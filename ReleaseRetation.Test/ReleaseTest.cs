using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NUnit.Framework;
using ReleaseRelation.WebApi;
using ReleaseRetation.Models;
using ReleaseRetation.Controllers;
using ReleaseRetation;
using System.Linq;
using Serilog;

namespace ReleaseRetation.Test
{
    [TestFixture]
    class ReleaseTest
    {
        [Test]
        public async Task OneRelease_KeepOne()
        {
            Log.Logger.Information("ReleaseTest: OneRelease_KeepOne Start Running");
            // ////Init
            var rig = await new SwaggerIntegration().Start();
            ReleaseApi rService = new ReleaseApi(rig.HttpTestClient);

            //Init test data: one project and one environment  
            ProjectRelease model = new ProjectRelease("Project-1", "Environment-1");

            Log.Logger.Information($"Test Data: {model.ProjectId} {model.EnvironmentId} {model.NumberOfReleases}");
            //Call WebApi
            var data =  await rService.GetReleasesAsync(model);
            var result = data.ToList();

            //Only one environment
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Count);
            });

            //Only release should be there
            var releases = result[0].ToList();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, releases.Count);
            });


            //Verify the kept release
            var releaseKept =(Release)releases[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Release-2", releaseKept.Id);
                Assert.AreEqual(1, releaseKept.Deployments.Count);
                Assert.AreEqual("Environment-1", releaseKept.Deployments[0].EnvironmentId);
            });

            Log.Logger.Information($"{releaseKept.Id} kept because it was the most recently deployed to {releaseKept.Deployments[0].Id}");
            Log.Logger.Information("ReleaseTest: OneRelease_KeepOne End");

        }

        [Test]
        public async Task TwoRelease_SameEnv_KeepOne()
        {
            Log.Logger.Information("ReleaseTest: TwoRelease_SameEnv_KeepOne Start Running");
            // ////Init
            var rig = await new SwaggerIntegration().Start();
            ReleaseApi rService = new ReleaseApi(rig.HttpTestClient);

            //Init test data: one project and one environment  
            ProjectRelease model = new ProjectRelease("Project-1", "Environment-1");
            Log.Logger.Information($"Test Data: {model.ProjectId} {model.EnvironmentId} {model.NumberOfReleases}");
            //Call WebApi
            var data = await rService.GetReleasesAsync(model);
            var result = data.ToList();

            //Only one environment
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Count);
            });

            //Only release should be there
            var releases = result[0].ToList();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, releases.Count);
            });


            //Verify the kept release
            var releaseKept = (Release)releases[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Release-2", releaseKept.Id);
                Assert.AreEqual(1, releaseKept.Deployments.Count);
                Assert.AreEqual("Environment-1", releaseKept.Deployments[0].EnvironmentId);
            });

            Log.Logger.Information($"{releaseKept.Id} kept because it was the most recently deployed to {releaseKept.Deployments[0].Id}");
            Log.Logger.Information("ReleaseTest: TwoRelease_SameEnv_KeepOne End");
        }

        [Test]
        public async Task TwoRelease_SameEnv_KeepTwo()
        {
            Log.Logger.Information("ReleaseTest: OneRelease_KeepOne Start Running");

            // ////Init
            var rig = await new SwaggerIntegration().Start();
            ReleaseApi rService = new ReleaseApi(rig.HttpTestClient);

            //Init test data: one project and one environment  
            ProjectRelease model = new ProjectRelease("Project-1", "Environment-1", 2);
            Log.Logger.Information($"Test Data: {model.ProjectId} {model.EnvironmentId} {model.NumberOfReleases}");

            //Call WebApi
            var data = await rService.GetReleasesAsync(model);
            var result = data.ToList();

            //Only one environment
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, result.Count);
            });

            //Only release should be there
            var releases = result[0].ToList();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, releases.Count);
            });


            //Verify the kept release
            var releaseKept1 = releases[0];
            var releaseKept2 = releases[1];
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Release-2", releaseKept1.Id);
                Assert.AreEqual(1, releaseKept1.Deployments.Count);
                Assert.AreEqual("Environment-1", releaseKept1.Deployments[0].EnvironmentId);

                Assert.AreEqual("Release-1", releaseKept2.Id);
                Assert.AreEqual(1, releaseKept2.Deployments.Count);
                Assert.AreEqual("Environment-1", releaseKept2.Deployments[0].EnvironmentId);
            });

            Log.Logger.Information($"{releaseKept1.Id} kept because it was the most recently deployed to {releaseKept1.Deployments[0].Id}");
            Log.Logger.Information($"{releaseKept2.Id} kept because it was the most recently deployed to {releaseKept2.Deployments[0].Id}");
            Log.Logger.Information("ReleaseTest: OneRelease_KeepOne End");
        }

        [Test]
        public async Task TwoRelease_TwoEnv_KeepOne()
        {
            Log.Logger.Information("ReleaseTest: OneRelease_KeepOne Start Running");
            // ////Init
            var rig = await new SwaggerIntegration().Start();
            ReleaseApi rService = new ReleaseApi(rig.HttpTestClient);

            //Init test data: one project and one environment  
            ProjectRelease model = new ProjectRelease("Project-1", "Environment-1", 1);
            model.Environments.Add("Environment-2");

            foreach(var e in model.Environments)
                Log.Logger.Information($"Test Data: {model.ProjectId} {e} {model.NumberOfReleases}");

            //Call WebApi
            var data = await rService.GetReleasesAsync(model);
            var result = data.ToList();

            //Only one environment
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, result.Count);
            });

            //Only release should be there
            var releases1 = result[0].ToList();
            var releases2 = result[1].ToList();
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, releases1.Count);
                Assert.AreEqual(1, releases2.Count);
            });


            //Verify the kept release
            var releaseKept1 = releases1[0];
            var releaseKept2 = releases2[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual("Release-2", releaseKept1.Id);
                Assert.AreEqual(1, releaseKept1.Deployments.Count);
                Assert.AreEqual("Environment-1", releaseKept1.Deployments[0].EnvironmentId);

                Assert.AreEqual("Release-1", releaseKept2.Id);
                Assert.AreEqual("Deployment-3", releaseKept2.Deployments[0].Id);
                Assert.AreEqual(1, releaseKept2.Deployments.Count);
                Assert.AreEqual("Environment-2", releaseKept2.Deployments[0].EnvironmentId);
            });


            Log.Logger.Information($"{releaseKept1.Id} kept because it was the most recently deployed to {releaseKept1.Deployments[0].Id}");
            Log.Logger.Information($"{releaseKept2.Id} kept because it was the most recently deployed to {releaseKept2.Deployments[0].Id}");
            Log.Logger.Information("ReleaseTest: OneRelease_KeepOne End");
        }
    }
}
