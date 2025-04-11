using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DB.Application.Features.Instruments.Commands.CreateInstrument
{
    public class CreateInstrumentCommandValidator : AbstractValidator<CreateInstrumentCommand>
    {
        public CreateInstrumentCommandValidator()
        {
            RuleFor(instrument => instrument.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }

}
