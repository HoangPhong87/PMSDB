using System.Web.Mvc;

namespace ProjectManagementSystem.Common
{
    public class StringModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            
            bindingContext.ModelMetadata.ConvertEmptyStringToNull = false;

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}