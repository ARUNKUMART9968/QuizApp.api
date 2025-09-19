using System.ComponentModel.DataAnnotations;

namespace QuizApp.Api.Validation
{
    public class ValidateOptionsAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is DTOs.CreateQuestionDto dto)
            {
                if (dto.Type == "MultipleChoice")
                {
                    if (dto.Options == null || dto.Options.Count < 2)
                    {
                        return new ValidationResult("Multiple choice questions must have at least 2 options");
                    }

                    if (!dto.Options.Contains(dto.CorrectAnswer))
                    {
                        return new ValidationResult("Correct answer must be one of the provided options");
                    }
                }
                else if (dto.Type == "TrueFalse")
                {
                    if (dto.CorrectAnswer != "True" && dto.CorrectAnswer != "False")
                    {
                        return new ValidationResult("True/False questions must have 'True' or 'False' as correct answer");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}