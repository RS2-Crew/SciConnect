using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB.Application.Features.Analyses.Commands.CreateAnalysis;
using FluentValidation;

namespace DB.Application.Features.Analysis.Commands.CreateAnalysis
{
    public class CreateAnalysisCommandValidator : AbstractValidator<CreateAnalysisCommand>
    {
        public CreateAnalysisCommandValidator()
        {
            RuleFor(instrument => instrument.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }

}

