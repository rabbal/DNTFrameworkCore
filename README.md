# DNTFrameworkCore 
[![Build status](https://rabbal.visualstudio.com/DNTFrameworkCore/_apis/build/status/DNTFrameworkCore-Master-CI)](https://rabbal.visualstudio.com/DNTFrameworkCore/_build/latest?definitionId=3)

## What is DNTFrameworkCore?

`DNTFrameworkCore` is a Lightweight and 
Extensible Infrastructure for Building High Quality Web Applications Based on ASP.NET Core and has the following goals:
* Common structures in various applications like Cross-Cutting Concerns and etc
* Follow DRY principle to focus on main business logic
* Reduce the development time
* Less bug and stop bug propagation 
* Reduce the training time of the new developer with low knowledge about OOP and OOD
 
 ![Blog CRUD API](https://github.com/rabbal/DNTFrameworkCore/tree/master/docs/blog-crud-api.jpg)
 
 ![Blog CRUD Service](https://github.com/rabbal/DNTFrameworkCore/tree/master/docs/blog-crud-service.jpg)
 
## Installation

To create your first project based on DNTFrameworkCore you can install the following packages:
```PM> Install-Package DNTFrameworkCore
PM> Install-Package DNTFrameworkCore.EntityFramework
PM> Install-Package DNTFrameworkCore.EntityFramework.SqlServer
PM> Install-Package DNTFrameworkCore.Web
PM> Install-Package DNTFrameworkCore.Web.EntityFramework
PM> Install-Package DNTFrameworkCore.FluentValidation
PM> Install-Package DNTFrameworkCore.Web.MultiTenancy
```

OR

1- Run the following command to install boilerplate project template based on ASP.NET Core Web API and DNTFrameworkCore:

```dotnet new --install DNTFrameworkCoreTemplateAPI::*‌‌```

2- Create new project with installed template:

```dotnet new dntcore-api```

Now you have a solution like below that contains complete identity management feature include user,role and dynamic permission management and also integrated with persistent JWT authentication machanism:

![Solution Structure](https://github.com/rabbal/DNTFrameworkCore/tree/master/docs/dnt-solution.jpg)

For more info about templates you can watch [DNTFrameworkCoreTemplate repository](https://github.com/rabbal/DNTFrameworkCoreTemplate)

## Features

* Automatic Input Validation and Business Validation
* Automatic Transaction Management
* Eventing 
* Aggregate Update (Master-Detail)
* Automatic Numbering
* Functional Programming Error Handling
* Dynamic Permission Authorization
* CrudService 
* CrudController
* DbLogger Provider
* Auditing
* DataProtectionKeys DbRepository 
* Name-Value Setting Management
* Hooks
* SoftDelete
* MultiTenancy
* Tracking mechanism (CreatorUserId,CreationDateTime,LastModifierUserId,...)
* FluentValidation Integration
* BackgroundTaskQueue
* CQRS (coming soon)
* EntityHistory (coming soon)

## Usage
[DNTFrameworkCore.TestAPI Complete ASP.NET Core Web API](https://github.com/rabbal/DNTFrameworkCore/tree/master/test/DNTFrameworkCore.TestAPI)

**Create Entity**
```c#
public class Task : TrackableEntity<int>, IAggregateRoot, INumberedEntity
{
    public const int MaxTitleLength = 256;
    public const int MaxDescriptionLength = 1024;

    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public string Number { get; set; }
    public string Description { get; set; }
    public TaskState State { get; set; } = TaskState.Todo;
    public byte[] RowVersion { get; set; }
}
```

**Implement ProjectDbContext that inherited from DbContextCore**
```c#
public class ProjectDbContext : DbContextCore
{
    public ProjectDbContext(DbContextCoreDependency<ProjectDbContext> dependency) : base(dependency)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaskConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    protected override void AfterSaveChanges(SaveChangeContext context)
    {
        //if you are using https://github.com/VahidN/EFSecondLevelCache.Core as SecondLevelCache library
        this.GetService<IEFCacheServiceProvider>()
            .InvalidateCacheDependencies(context.ChangedEntityNames.ToArray());
    }
}
```
**Create Model/DTO**
```c#
[LocalizationResource(Name = "SharedResource", Location = "DNTFrameworkCore.TestAPI")]
public class TaskModel : MasterModel<int>, IValidatableObject
{
    public string Title { get; set; }

    [MaxLength(50, ErrorMessage = "Validation from DataAnnotations")]
    public string Number { get; set; }

    public string Description { get; set; }
    public TaskState State { get; set; } = TaskState.Todo;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Title == "IValidatableObject")
        {
            yield return new ValidationResult("Validation from IValidatableObject");
        }
    }
}
```

Note: Based on validation infrastructure, you can validate Model/DTO with various approach, using by DataAnnotation ValidateAttribute, Implementing IValidatableObject or Implement IModelValidator<T> that exist in DNTFrameworkCore package.

```c#
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
```

Also in most cases, one Model/DTO can be enough for your requirements about Create/Edit/View an entity. However you can create ReadModel like below:
```c#
public class TaskReadModel : MasterModel<int>
{
    public string Title { get; set; }
    public string Number { get; set; }
    public TaskState State { get; set; } = TaskState.Todo;
}
```

**Implement Service**
 
```c#
public interface ITaskService : ICrudService<int, TaskReadModel, TaskModel, TaskFilteredPagedQueryModel>
{
}

public class TaskService : CrudService<Task, int, TaskReadModel, TaskModel, TaskFilteredPagedQueryModel>,
    ITaskService
{
    private readonly IMapper _mapper;

    public TaskService(CrudServiceDependency dependency, IMapper mapper) : base(dependency)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    protected override IQueryable<TaskReadModel> BuildReadQuery(TaskFilteredPagedQueryModel model)
    {
        return EntitySet.AsNoTracking()
            .WhereIf(model.State.HasValue, t => t.State == model.State)
        	.ProjectTo<TaskReadModel>(_mapper.ConfigurationProvider);
    }

    protected override Task MapToEntity(TaskModel model)
    {
        return _mapper.Map<Task>(model);
    }

    protected override TaskModel MapToModel(Task entity)
    {
        return _mapper.Map<TaskModel>(entity);
    }
}
```

In DNTFrameworkCore.EntityFramework [there is no dependency to AutoMapper](https://cezarypiatek.github.io/post/why-i-dont-use-automapper/) or other mapper libraries, then you can do mapping between Entity and Model manually by implementing MapToModel and MapToEntity abstract methods.

**Implement API Controller**
```c#
[Route("api/[controller]")]
public class
    TasksController : CrudController<ITaskService, int, TaskReadModel, TaskModel, TaskFilteredPagedQueryModel>
{
    public TasksController(ITaskService service) : base(service)
    {
    }

    protected override string CreatePermissionName => PermissionNames.Tasks_Create;
    protected override string EditPermissionName => PermissionNames.Tasks_Edit;
    protected override string ViewPermissionName => PermissionNames.Tasks_View;
    protected override string DeletePermissionName => PermissionNames.Tasks_Delete;
}
```

```c#

[Route("api/[controller]")]
public class BlogsController : CrudController<IBlogService, int, BlogModel>
{
    public BlogsController(IBlogService service) : base(service)
    {
    }

    protected override string CreatePermissionName => PermissionNames.Blogs_Create;
    protected override string EditPermissionName => PermissionNames.Blogs_Edit;
    protected override string ViewPermissionName => PermissionNames.Blogs_View;
    protected override string DeletePermissionName => PermissionNames.Blogs_Delete;
}

```
