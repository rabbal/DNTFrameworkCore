# DNTFrameworkCore 


### What is DNTFrameworkCore?

`DNTFrameworkCore` is a Lightweight and 
Extensible Infrastructure for Building High Quality Web Applications Based on ASP.NET Core and has the following goals:
* Common structures in various applications like Cross-Cutting Concerns and etc
* Follow DRY principle to focus on main business logic
* Reduce the development time
* Less bug and stop bug propagation 
* Reduce the training time of the new developer with low knowledge about OOP and OOD
 
Application Service
```csharp
public interface IBlogService : ICrudService<int, BlogModel>
{
}

public class BlogService : CrudService<Blog, int, BlogModel>, IBlogService
{
    private readonly IMapper _mapper;

    public BlogService(
        IUnitOfWork uow,
        IEventBus bus,
        IMapper mapper) : base(uow, bus)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override Task<IPagedResult<BlogModel>> ReadPagedListAsync(FilteredPagedRequest request,
        CancellationToken cancellationToken = default)
    {
        return EntitySet.AsNoTracking()
            .Select(b => new BlogModel
            {
                Id = b.Id,
                Version = b.Version,
                Url = b.Url,
                Title = b.Title
            }).ToPagedListAsync(request, cancellationToken);
    }

    protected override void MapToEntity(BlogModel model, Blog blog)
    {
        _mapper.Map(model, blog);
    }

    protected override BlogModel MapToModel(Blog blog)
    {
        return _mapper.Map<BlogModel>(blog);
    }
}
 ``` 
 
ASP.NET Core WebAPI
```csharp
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
 
 ASP.NET Core MVC
 ```csharp
public class BlogsController : CrudController<IBlogService, int, BlogModel>
{
    public BlogsController(IBlogService service) : base(service)
    {
    }

    protected override string CreatePermissionName => PermissionNames.Blogs_Create;
    protected override string EditPermissionName => PermissionNames.Blogs_Edit;
    protected override string ViewPermissionName => PermissionNames.Blogs_View;
    protected override string DeletePermissionName => PermissionNames.Blogs_Delete;
    protected override string ViewName => "_BlogPartial";
}
 ```
 
 _BlogPartial.cshtml
 ```razor
@inherits EntityFormRazorPage<BlogModel>
@{
    Layout = "_EntityFormLayout";
    EntityName = "Blog";
    DeletePermission = PermissionNames.Blogs_Delete;
    CreatePermission = PermissionNames.Blogs_Create;
    EditPermission = PermissionNames.Blogs_Edit;
    EntityDisplayName = "Blog";
}

<div class="form-group row">
    <div class="col col-md-8">
        <label asp-for="Title" class="col-form-label text-md-left"></label>
        <input asp-for="Title" autocomplete="off" class="form-control"/>
        <span asp-validation-for="Title" class="text-danger"></span>
    </div>
</div>
<div class="form-group row">
    <div class="col">
        <label asp-for="Url" class="col-form-label text-md-left"></label>
        <input asp-for="Url" class="form-control" type="url"/>
        <span asp-validation-for="Url" class="text-danger"></span>
    </div>
</div>

```
 ![Role Modal MVC](https://github.com/rabbal/DNTFrameworkCore/blob/master/docs/role-modal-edit.JPG)
## Installation

To create your first project based on DNTFrameworkCore you can install the following packages:
```
PM> Install-Package DNTFrameworkCore
PM> Install-Package DNTFrameworkCore.EFCore
PM> Install-Package DNTFrameworkCore.EFCore.SqlServer
PM> Install-Package DNTFrameworkCore.Web
PM> Install-Package DNTFrameworkCore.Web.Tenancy
PM> Install-Package DNTFrameworkCore.Web.EFCore
PM> Install-Package DNTFrameworkCore.Licensing
PM> Install-Package DNTFrameworkCore.FluentValidation

```

OR

1- Run the following command to install boilerplate project template based on ASP.NET Core Web API and DNTFrameworkCore:

```dotnet new --install DNTFrameworkCoreTemplateAPI::*‌‌```

2- Create new project with installed template:

```dotnet new dntcore-api```

Now you have a solution like below that contains complete identity management feature include user,role and dynamic permission management and also integrated with persistent JWT authentication machanism:

![Solution Structure](https://github.com/rabbal/DNTFrameworkCore/blob/master/docs/dnt-solution.jpg)

For more info about templates you can watch [DNTFrameworkCoreTemplate repository](https://github.com/rabbal/DNTFrameworkCoreTemplate)

## Features

* Application Input Validation
* Transaction Management
* Eventing
* EntityGraph Tracking (Master-Detail)
* Numbering
* Functional Programming Error Handling
* Permission Authorization
* CrudService
* CrudController (API and MVC)
* DbLogger Provider based on EFCore
* ProtectionKey EFCore Store
* Hooks
* SoftDelete
* Tenancy
* Tracking mechanism (ICreationTracking, IModificationTracking)
* FluentValidation Integration
* BackgroundTaskQueue
* RowIntegrity
* StartupTask mechanism
* CQRS (coming soon)
* EntityHistory (coming soon)

## Usage
[DNTFrameworkCore.TestAPI Complete ASP.NET Core Web API](https://github.com/rabbal/DNTFrameworkCore/tree/master/test/DNTFrameworkCore.TestAPI)

**Create Entity**
```c#
public class Task : Entity<int>, INumberedEntity, IHasRowVersion, IHasRowIntegrity, ICreationTracking, IModificationTracking
{
    public const int MaxTitleLength = 256;
    public const int MaxDescriptionLength = 1024;

    public string Title { get; set; }
    public string NormalizedTitle { get; set; }
    public string Number { get; set; }
    public string Description { get; set; }
    public TaskState State { get; set; } = TaskState.Todo;
    
    public byte[] Version { get; set; }
    public string Hash { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime? ModifiedDateTime { get; set; }
}
```

**Implement ProjectDbContext that inherited from DbContextCore**
```c#
public class ProjectDbContext : DbContextCore
{
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options, IEnumerable<IHook> hooks) : base(options, hooks)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {               
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.AddJsonFields();
        modelBuilder.AddTrackingFields<long>();
        modelBuilder.AddIsDeletedField();
        modelBuilder.AddRowVersionField();
        modelBuilder.AddRowIntegrityField();
            
        modelBuilder.NormalizeDateTime();
        modelBuilder.NormalizeDecimalPrecision();
            
        base.OnModelCreating(modelBuilder);
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
public class TaskReadModel : ReadModel<int>
{
    public string Title { get; set; }
    public string Number { get; set; }
    public TaskState State { get; set; } = TaskState.Todo;
}
```

**Implement Service**
 
```c#
public interface ITaskService : ICrudService<int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
{
}

public class TaskService : CrudService<Task, int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>,
    ITaskService
{
    private readonly IMapper _mapper;
    public TaskService(IUnitOfWork uow, IEventBus bus, IMapper mapper) :base(uow, bus)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper);
    }

    public override Task<IPagedResult<TaskReadModel>> ReadPagedListAsync(TaskFilteredPagedRequest request,
        CancellationToken cancellationToken = default)
    {
        return EntitySet.AsNoTracking()
            .WhereIf(model.State.HasValue, t => t.State == model.State)
            .Select(t => new TaskReadModel
            {
                Id = t.Id,
                Title = t.Title,
                State = t.State,
                Number = t.Number
            }).ToPagedListAsync(request, cancellationToken);
    }

    protected override void MapToEntity(TaskModel model, Task task)
    {
        _mapper.Map(model, task);
    }

    protected override TaskModel MapToModel(Task task)
    {
        return _mapper.Map<TaskModel>(task);
    }
}
```

In DNTFrameworkCore.EFCore [there is no dependency to AutoMapper](https://cezarypiatek.github.io/post/why-i-dont-use-automapper/) or other mapper libraries, then you can do mapping between Entity and Model manually by implementing MapToModel and MapToEntity abstract methods.

**Implement API Controller**
```c#
[Route("api/[controller]")]
public class
    TasksController : CrudController<ITaskService, int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
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

## ASP.NET Boilerplate
A small part of this project like the following sections are taken from [ABP](https://github.com/aspnetboilerplate/aspnetboilerplate)
- Validation with refactoring to support functional programming error handling mechanism
