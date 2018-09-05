using AcmeContract.FileModels;
using AcmeContract.Models;
using AutoMapper;
using System;

namespace AcmeContract.Mapping
{
    public class AutoMapper : Profile
    {

        public AutoMapper()
        {
            CreateMap<CsvContact, ContactModel>();
           
        }
       
    }
}