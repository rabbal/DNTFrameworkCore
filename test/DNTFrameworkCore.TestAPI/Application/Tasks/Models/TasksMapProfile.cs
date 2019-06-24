using AutoMapper;
using DNTFrameworkCore.TestAPI.Domain.Tasks;
using DNTFrameworkCore.TestAPI.Helpers;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
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