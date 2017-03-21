using System;
using AutoMapper;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Commands;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders.Mappings
{
    public class AutomapperMappingProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Patient, PatientViewModel>()
                .Include<Patient, EditPatientViewModel>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(source => source.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(source => source.LastName))
                .ForMember(dest => dest.NhsNumber, opt => opt.MapFrom(source => source.NhsNumber))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(source => source.PatientId))
                .ForMember(dest => dest.ClinicalSystemId, opt => opt.MapFrom(source => source.ClinicalSystemId))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(source => source.GenderId))
                .ForMember(dest => dest.DateOfBirthViewModel,
                    opt =>
                        opt.MapFrom(
                            source =>
                                new DateOfBirthViewModel()
                                {
                                    Day = source.DateOfBirth.Day,
                                    Month = source.DateOfBirth.Month,
                                    Year = source.DateOfBirth.Year
                                }));

            Mapper.CreateMap<Patient, EditPatientViewModel>();

            Mapper.CreateMap<PatientViewModel, AddUpdatePatientCommand>()
                .Include<EditPatientViewModel, AddUpdatePatientCommand>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(source => source.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(source => source.LastName))
                .ForMember(dest => dest.NhsNumber, opt => opt.MapFrom(source => source.NhsNumber))
                .ForMember(dest => dest.ClinicalSystemId, opt => opt.MapFrom(source => source.ClinicalSystemId))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(source => source.GenderId))
                .ForMember(dest => dest.DateOfBirth,
                    opt =>
                        opt.MapFrom(
                            source =>
                                new DateTime(source.DateOfBirthViewModel.Year.Value, source.DateOfBirthViewModel.Month.Value, source.DateOfBirthViewModel.Day.Value)))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            Mapper.CreateMap<EditPatientViewModel, AddUpdatePatientCommand>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(source => source.PatientId));
        }
    }
}