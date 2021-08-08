using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NUnit.Framework;

namespace ReleaseRetation.Test
{
    [TestFixture]
    class GenerateAPIs
    {
        [Test]
        public async Task Generate()
        {
            var rig = await new SwaggerIntegration().Start();
            try
            {
                //System.Threading.Thread.Sleep(10000);
                var response = await rig.HttpTestClient.GetAsync("/swagger/v1/swagger.json");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                var document = await OpenApiDocument.FromJsonAsync(responseString);

                var settings = new CSharpClientGeneratorSettings
                {
                    ClassName = "{controller}Api",
                    AdditionalContractNamespaceUsages = new string[] { "" },
                    AdditionalNamespaceUsages = new string[] { "ReleaseRetation.Controllers", "ReleaseRetation.Models", "Microsoft.AspNetCore.Mvc.ModelBinding", "Microsoft.AspNetCore.Mvc" },
                    CSharpGeneratorSettings =
                    {
                        Namespace = "ReleaseRelation.WebApi"
                    },
                    GenerateClientClasses = true,
                    GenerateResponseClasses = false,
                    GenerateDtoTypes = false
                };

                var generator = new CSharpClientGenerator(document, settings);
                var code = generator.GenerateFile();
                var json = document.ToJson();

                json = json.Replace("localhost", "swagger: 2.0");
                string path = System.IO.Path.Join(Environment.CurrentDirectory, "..//..//..//WebAPI.cs");
                System.IO.File.WriteAllText(path, code);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception:" + ex.Message);
            }
            finally
            {
                rig.Stop();
            }
        }
    }
}
