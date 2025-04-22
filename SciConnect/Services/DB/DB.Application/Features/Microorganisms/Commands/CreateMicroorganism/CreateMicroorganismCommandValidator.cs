using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DB.Application.Features.Microorganisms.Commands.CreateMicroorganism
{
    public class CreateMicroorganismCommandValidator : AbstractValidator<CreateMicroorganismCommand>
    {
        public CreateMicroorganismCommandValidator()
        {
            RuleFor(microorganism => microorganism.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }

}
