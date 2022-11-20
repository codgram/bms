using Application.Server.Data;
using Application.Model.Org;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Application.Server.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class CustomAuthorize : AuthorizeAttribute, IAsyncActionFilter
    {

        private readonly string _roleNames;
        
        public CustomAuthorize(string roleNames)
        {
            _roleNames = roleNames;
        }
        private const string COMPANYID = "companyId";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.HttpContext.Request.Query.TryGetValue(COMPANYID, out var extractedCompanyId))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Company Id was not provided"
                };
                return;
            }

            var company = await context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>().Company.FindAsync(extractedCompanyId);

            var isRoleCorrect = false;

            // check if the role is correct
            foreach (var role in _roleNames.Split(','))
            {
                if (context.HttpContext.User.IsInRole($"{company.Code}_{role}"))
                {
                    isRoleCorrect = true;
                    break;
                }
            }      

            if(!isRoleCorrect)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "You are not authorized to perform this action"
                };

                return;
            }
            

            await next();
        }


    }
}