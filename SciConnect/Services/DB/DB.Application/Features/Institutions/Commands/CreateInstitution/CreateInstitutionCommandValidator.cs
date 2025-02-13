using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DB.Application.Features.Institutions.Commands.CreateInstitution
{
    public class CreateInstitutionCommandValidator : AbstractValidator<CreateInstitutionCommand>
    {
        public CreateInstitutionCommandValidator()
        {
            RuleFor(institution => institution.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{Name} must not exceed 50 caracters.");
        }
    }
}
