using System.ComponentModel.DataAnnotations;

namespace MyBGList.Attributes
{
    public class SortColumnValidatorAttribute : ValidationAttribute
    {
        public Type EntityType { get; set; }

        public SortColumnValidatorAttribute(Type entityType) : base("Value must match an existing column")
        {
            EntityType = entityType;
        }

        // Need to research LINQ methods (GetProperties, ANY, etc) more and learn more about customizing Validators. Learn more about ...
        // ValidationResult as well. (GetProperties returns an array of PropertyInfo Objects.)
        // IsValid isn't the best case use, should use DTO's instead of entity classes to ensure safety of data (User doesn't
        // have access to data that they shouldn't). In this case, using IsValid for learning.
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (EntityType != null)
            {
                var strValue = value as string;
                if (!string.IsNullOrEmpty(strValue) && EntityType.GetProperties().Any(p => p.Name == strValue))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
