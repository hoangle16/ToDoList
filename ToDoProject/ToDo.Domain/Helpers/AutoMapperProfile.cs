using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo.Domain.Entities;
using ToDo.Domain.Models;
using ToDo.Domain.Models.ToDoItem;

namespace ToDo.Domain.Helpers
{
    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            //items
            CreateMap<ToDoItem, ItemModel>();
            CreateMap<AddItemModel, ToDoItem>();
            CreateMap<UpdateItemModel, ToDoItem>();
        }
    }
}
