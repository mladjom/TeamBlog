﻿using System.Threading.Tasks;
using TeamBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace TeamBlog.Authorization
{
    public class ArticleEditorAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Article>
    {
        protected override Task
            HandleRequirementAsync(
                AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Article resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can approve or reject.
            if (context.User.IsInRole(Constants.ArticleEditorRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
