using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class TemplateService : ITemplateService
    {
        public Task<string> GetTemplateAsync(string templateName)
        {
            // Tải template từ file hệ thống, cơ sở dữ liệu, hoặc nguồn khác
            // Ví dụ: tải từ file
            var template = System.IO.File.ReadAllText($"Templates/{templateName}.html");
            return Task.FromResult(template);
        }

        public string RenderTemplate(string template, object model)
        {
            var engine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();

            return engine.CompileRenderStringAsync("templateKey", template, model).Result;
        }
    }
}
