using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MyBGList.Attributes
{
    public class ManualValidationFilterAttribute : Attribute, IActionModelConvention
    {
        // ModelStateInvalidFilterFactory type is marked internal, which prevents us from checking for the filter pressence by using a 
        // strongly typed approach. We must compare the name property with the literal name of the class.
        public void Apply(ActionModel action)
        {
            for (var i = 0; i < action.Filters.Count; i++)
            {
                if (action.Filters[i] is ModelStateInvalidFilter || action.Filters[i].GetType().Name == "ModelStateInvalidFilterFactory")
                {
                    action.Filters.RemoveAt(i);
                    break;
                }
            }
            throw new NotImplementedException();
        }
    }
}
