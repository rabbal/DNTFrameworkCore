<img alt="logo" src="docs/logo.png" height="64"/>


[![.NET](https://github.com/rabbal/DNTFrameworkCore/actions/workflows/dotnet.yml/badge.svg)](https://github.com/rabbal/DNTFrameworkCore/actions/workflows/dotnet.yml)
![Nuget](https://img.shields.io/nuget/v/DNTFrameworkCore)
[![build aspnet-core-api template](https://github.com/rabbal/DNTFrameworkCore/actions/workflows/aspnet-core-api-template.yml/badge.svg)](https://github.com/rabbal/DNTFrameworkCore/actions/workflows/aspnet-core-api-template.yml)
![Nuget](https://img.shields.io/nuget/v/DNTFrameworkCoreTemplateAPI?label=aspnet-core-api-template)
### What is DNTFrameworkCore?

`DNTFrameworkCore` is a Lightweight and 
Extensible Infrastructure for Building High-Quality Web Applications Based on ASP.NET Core and has the following goals:
* Common structures in various applications like Cross-Cutting Concerns, etc
* Follow DRY principle to focus on main business logic
* Reduce the development time
* Less bug and stop bug propagation 
* Reduce the training time of the new developer with low knowledge about OOP and OOD

CRUD-based Thinking
----

Application Service
```c#
public interface IBlogService : IEntityService<int, BlogModel>
{
}

public class BlogService : EntityService<Blog, int, BlogModel>, IBlogService
{
    private readonly IMapper _mapper;

    public BlogService(
        IDbContext dbContext,
        IEventBus bus,
        IMapper mapper) : base(dbContext, bus)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override Task<IPagedResult<BlogModel>> FetchPagedListAsync(FilteredPagedRequest request,
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
```c#
[Route("api/[controller]")]
public class BlogsController : EntityController<IBlogService, int, BlogModel>
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
 ```c#
public class BlogsController : EntityController<IBlogService, int, BlogModel>
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

Now you have a solution like below that contains complete identity management feature includes user, role, and dynamic permission management and also integrated with persistent JWT authentication mechanism:

![Solution Structure](https://github.com/rabbal/DNTFrameworkCore/blob/master/docs/dnt-solution.jpg)

For more info about templates, you can watch [DNTFrameworkCoreTemplate repository](https://github.com/rabbal/DNTFrameworkCoreTemplate)

## Features

* Application Input Validation
* Transaction Management
* Eventing
* EntityGraph Tracking (Master-Detail)
* Numbering
* Functional Programming Error Handling
* Permission Authorization
* EntityService
* EntityController (API and MVC)
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

Note: Based on validation infrastructure, you can validate Model/DTO with various approaches, using by DataAnnotation ValidateAttribute, Implementing IValidatableObject, or Implement IModelValidator<T> that exist in DNTFrameworkCore package.

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

Also in most cases, one Model/DTO can be enough for your requirements about Create/Edit/View an entity. However, you can create ReadModel like below:
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
public interface ITaskService : IEntityService<int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
{
}

public class TaskService : EntityService<Task, int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>,
    ITaskService
{
    private readonly IMapper _mapper;
    public TaskService(IDbContext dbContext, IEventBus bus, IMapper mapper) :base(dbContext, bus)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper);
    }

    public override Task<IPagedResult<TaskReadModel>> FetchPagedListAsync(TaskFilteredPagedRequest request,
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
    TasksController : EntityController<ITaskService, int, TaskReadModel, TaskModel, TaskFilteredPagedRequest>
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
public class BlogsController : EntityController<IBlogService, int, BlogModel>
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

Task-based Thinking
---------------
Rich Domain Model
```c#
public class PriceType : Entity<long>, IAggregateRoot
{
    private PriceType(Title title)
    {
        Title = title;
    }
    
    public PriceType(Title title, IPriceTypePolicy policy)
    {
        if (title == null) throw new ArgumentNullException(nameof(title));
        if (policy == null) throw new ArgumentNullException(nameof(policy));

        Title = title;

        if (!policy.IsUnique(this)) ThrowRuleException("PriceType Title Should Be Unique");

        AddDomainEvent(new PriceTypeCreatedDomainEvent(this));
    }

    public Title Title { get; private set; }

    // public static Result<PriceType> New(Title title, IPriceTypePolicy policy)
    // {
    //     if (title == null) throw new ArgumentNullException(nameof(title));
    //     if (policy == null) throw new ArgumentNullException(nameof(policy));
    //
    //     var priceType = new PriceType(title);
    //     if (!policy.IsUnique(priceType)) return Fail<PriceType>("PriceType Title Should Be Unique");
    //
    //     priceType.AddDomainEvent(new PriceTypeCreatedDomainEvent(priceType));
    //
    //     return Ok(priceType);
    // }
}
```
ValueObject
```c#
public class Title : ValueObject
{
    private Title()
    {
    }

    public Title(string value)
    {
        value ??= string.Empty;

        if (value.Length == 0) throw new BusinessRuleException("title should not be empty");

        if (value.Length > 100) throw new BusinessRuleException("title is too long");
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> EqualityValues
    {
        get { yield return Value; }
    }

    // public static Result<Title> New(string value)
    // {
    //     value ??= string.Empty;
    //
    //     if (value.Length == 0) return Fail<Title>("title should not be empty");
    //
    //     return value.Length > 100 ? Fail<Title>("title is too long") : Ok(new Title { Value = value });
    // }

    public static implicit operator string(Title title)
    {
        return title.Value;
    }

    public static explicit operator Title(string title)
    {
        return new(title);
    }
}
```
DomainEvent

```c#
public sealed class PriceTypeCreatedDomainEvent : DomainEvent
{
    public PriceTypeCreatedDomainEvent(PriceType priceType)
    {
        PriceType = priceType ?? throw new ArgumentNullException(nameof(priceType));
    }

    public PriceType PriceType { get; }
}
```

CQRS (Command)
```c#
public sealed class CreatePriceTypeCommand : ICommand
{
    public string Title { get; }
    [JsonConstructor]
    public CreatePriceTypeCommand(string title) => Title = title;
}
```
CQRS (CommandHandler)
```c#
public class PriceTypeCommandHandlers : ICommandHandler<RemovePriceTypeCommand, Result>,
   ICommandHandler<CreatePriceTypeCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IPriceTypeRepository _repository;
    private readonly IPriceTypePolicy _policy;
    private readonly IEventBus _bus;

    public PriceTypeCommandHandlers(
        IUnitOfWork uow,
        IPriceTypeRepository repository,
        IPriceTypePolicy policy,
        IEventBus bus)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public async Task<Result> Handle(RemovePriceTypeCommand command, CancellationToken cancellationToken)
    {
        var priceType = await _repository.FindAsync(command.PriceTypeId, cancellationToken);
        if (priceType is null) return Result.Fail($"PriceType with id:{command.PriceTypeId} not found");

        //Alternative: _uow.Set<PriceType>().Remove(priceType);
        _repository.Remove(priceType);

        await _uow.SaveChanges(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> Handle(CreatePriceTypeCommand command, CancellationToken cancellationToken)
    {
        var title = new Title(command.Title);

        var priceType = new PriceType(title, _policy);

        //Alternative: _uow.Set<PriceType>().Add(priceType);
        _repository.Add(priceType);
        
        await _uow.SaveChanges(cancellationToken);

        await _bus.DispatchDomainEvents(priceType, cancellationToken);

        return Result.None;
    }
}
```

## ASP.NET Boilerplate
[DNTFrameworkCore vs ABP Framework](https://medium.com/@rabbal/dntframeworkcore-vs-abp-framework-b48f5b7f8a24)

A small part of this project like the following sections are taken from [ABP](https://github.com/aspnetboilerplate/aspnetboilerplate)
- Validation with refactoring to support functional programming error handling mechanism
