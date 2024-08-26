﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface ITemplateService
    {
        Task<string> GetTemplateAsync(string templateName);
        string RenderTemplate(string template, object model);
    }
}
