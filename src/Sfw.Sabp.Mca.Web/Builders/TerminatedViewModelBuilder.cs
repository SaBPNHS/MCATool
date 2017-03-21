using AutoMapper;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class TerminatedViewModelBuilder : ITerminatedViewModelBuilder
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        
        public TerminatedViewModelBuilder(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public TerminatedViewModel BuildTerminatedAssessmentViewModel(Assessment assessment)
        {
            if (assessment == null) throw new ArgumentNullException("assessment");

            var viewModel = Mapper.DynamicMap<Assessment, TerminatedViewModel>(assessment);
            
            if (viewModel.DateAssessmentEnded == DateTime.MinValue)
            {
                viewModel.DateAssessmentEnded = _dateTimeProvider.Now;
            }

            return viewModel;
        }
    }
}