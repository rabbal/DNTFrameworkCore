using AutoMapper;
using DNTFrameworkCore.TestWebApp.Domain.Tasks;
using DNTFrameworkCore.TestWebApp.Helpers;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks.Models
{
    public class TasksMapProfile : Profile
    {
        public TasksMapProfile()
        {
            CreateMap<Task, TaskReadModel>(MemberList.None);
            
            CreateMap<TaskModel, Task>(MemberList.None)
                .ForMember(d => d.NormalizedTitle, m => m.MapFrom(s => s.Title.NormalizePersianTitle()))
                .ReverseMap();
        }
    }
}