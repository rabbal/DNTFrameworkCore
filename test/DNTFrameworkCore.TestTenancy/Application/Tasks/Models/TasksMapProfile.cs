using AutoMapper;
using DNTFrameworkCore.TestTenancy.Domain.Tasks;
using DNTFrameworkCore.TestTenancy.Helpers;

namespace DNTFrameworkCore.TestTenancy.Application.Tasks.Models
{
    public class TasksMapProfile : Profile
    {
        public TasksMapProfile()
        {
            //Required for ModifiedProperties collection of TrackableEntity and Model
            AllowNullCollections = true;

            CreateMap<TaskModel, Task>(MemberList.None)
                .ForMember(d => d.NormalizedTitle, m => m.MapFrom(s => s.Title.NormalizePersianTitle()))
                .ReverseMap();
        }
    }
}