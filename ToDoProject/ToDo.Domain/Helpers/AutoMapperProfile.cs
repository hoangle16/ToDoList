using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;
using ToDo.Domain.Models;

namespace ToDo.Domain.Helpers
{
    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}
