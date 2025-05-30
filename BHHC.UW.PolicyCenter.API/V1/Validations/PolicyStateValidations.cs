using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using FluentValidation;

namespace HC.UW.PolicyCenter.API.V1.Validations
{

    public class GetPolicyStatesQueryValidator : AbstractValidator<GetPolicyStatesQuery>
    {
        public GetPolicyStatesQueryValidator()
        {
            RuleFor(x => x.MgaCode)
                .NotEmpty().WithMessage("MGA Code is required.")
                .MaximumLength(10).WithMessage("MGA Code cannot be longer than 10 characters.");
        }
    }

    public class UpsertPolicyStateCommandRequestValidator : AbstractValidator<UpsertPolicyStateCommandRequest>
    {
        public UpsertPolicyStateCommandRequestValidator()
        {
            RuleFor(x => x.MgaCode)
                .NotEmpty().WithMessage("MGA Code is required.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.");
        }
    }

    public class GetAvailableStatesQueryValidator : AbstractValidator<GetAvailableStatesQuery>
    {
        public GetAvailableStatesQueryValidator()
        {
            RuleFor(x => x.MgaCode)
                .NotEmpty().WithMessage("MGA Code is required.")
                .MaximumLength(10).WithMessage("MGA Code cannot be longer than 10 characters.");
        }
    }
    //please register in program.cs
}
