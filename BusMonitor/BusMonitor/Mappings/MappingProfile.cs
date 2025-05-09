using AutoMapper;
using BusMonitor.DTOs;
using BusMonitor.Models;
using System;
using System.Linq;

namespace BusMonitor.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            
            CreateMap<CreateUpdateUserDTO, User>()
                .ForMember(dest => dest.Role, opt => 
                    opt.MapFrom(src => Enum.Parse<Role>(src.Role)));

            CreateMap<User, CreateUpdateUserDTO>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<Student, StudentDTO>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent.Name));

            CreateMap<Student, StudentDTO>().ReverseMap();

            CreateMap<Trip, TripDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.BusNumber, opt => opt.MapFrom(src => src.Bus.Number))
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => $"{src.Route.StartPoint} - {src.Route.EndPoint}"))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.Name : null))
                .ForMember(dest => dest.SupervisorName, opt => opt.MapFrom(src => src.Supervisor != null ? src.Supervisor.Name : null))
                .ForMember(dest => dest.AdminName, opt => opt.MapFrom(src => src.Admin != null ? src.Admin.Name : null))
                .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.StudentTrips.Select(st => st.Student)));

            CreateMap<TripDTO, Trip>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<Status>(src.Status)))
                .ForPath(dest => dest.Bus.Number, opt => opt.Ignore());

            CreateMap<CreateTripDTO, Trip>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Status.Planned));

            CreateMap<UpdateTripDTO, Trip>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<Status>(src.Status)));

            CreateMap<StudentReport, StudentReportDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.Name))
                .ForMember(dest => dest.BehaviorCategory, opt => opt.MapFrom(src => src.BehaviorCategory.ToString()));

            CreateMap<CreateStudentReportDTO, StudentReport>()
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BehaviorCategory, opt => 
                    opt.MapFrom(src => Enum.Parse<BehaviorCategory>(src.BehaviorCategory)));

            CreateMap<DelayReport, DelayReportDTO>();
            CreateMap<CreateDelayReportDTO, DelayReport>();

            CreateMap<Notification, NotificationDTO>();
        }
    }
} 