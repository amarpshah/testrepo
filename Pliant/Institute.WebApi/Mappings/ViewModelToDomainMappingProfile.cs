using AutoMapper;
using Institute.Entities;
using Institute.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public virtual string ProfileName
        {
             get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
            //Mapper.CreateMap<MovieViewModel, Movie>()
            //    //.ForMember(m => m.Image, map => map.Ignore())
            //    .ForMember(m => m.Genre, map => map.Ignore());
        }
    }
}