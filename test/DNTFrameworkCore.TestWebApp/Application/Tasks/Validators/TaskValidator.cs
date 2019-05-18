using System;
using System.Collections.Generic;
using DNTFrameworkCore.TestWebApp.Application.Tasks.Models;
using DNTFrameworkCore.TestWebApp.Domain.Tasks;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks.Validators
{
    public class TaskValidator : ModelValidator<TaskModel>
         {
             public override IEnumerable<ValidationFailure> Validate(TaskModel model)
             {
                 if (!Enum.IsDefined(typeof(TaskState), model.State))
                 {
                     yield return new ValidationFailure(nameof(TaskModel.State), "Validation from IModelValidator");
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