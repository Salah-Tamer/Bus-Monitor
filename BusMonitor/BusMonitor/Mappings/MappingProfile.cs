using AutoMapper;
using BusMonitor.DTOs;
using BusMonitor.Models;
using System;

namespace BusMonitor.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Role, opt => 
                    opt.MapFrom(src => Enum.Parse<Role>(src.Role)));
        }
    }
} 