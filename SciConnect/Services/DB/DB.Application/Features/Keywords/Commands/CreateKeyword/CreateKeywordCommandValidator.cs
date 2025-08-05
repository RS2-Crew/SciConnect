using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DB.Application.Features.Keywords.Commands.CreateKeyword
{
    public class CreateKeywordCommandValidator : AbstractValidator<CreateKeywordCommand>
    {
        public CreateKeywordCommandValidator()
        {
            RuleFor(keyword => keyword.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
        }
    }
}
