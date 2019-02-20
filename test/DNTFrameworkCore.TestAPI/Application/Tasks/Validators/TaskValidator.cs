using System;
using System.Collections.Generic;
using DNTFrameworkCore.TestAPI.Application.Tasks.Models;
using DNTFrameworkCore.TestAPI.Domain.Tasks;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Validators
{
    public class TaskValidator : ModelValidator<TaskModel>
         {
             public override IEnumerable<ModelValidationResult> Validate(TaskModel model)
             {
                 if (!Enum.IsDefined(typeof(TaskState), model.State))
                 {
                     yield return new ModelValidationResult(nameof(TaskModel.State), "Validation from IModelValidator");
                 }
             }
         }

    // FluentValidation Library can be used 
    // public class TaskFluentValidator : FluentModelValidator<TaskModel>
    //{
    //  public TaskFluentValidator()
    //  {
    //  }
    //}
}