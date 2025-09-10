using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SMS.Web;

[HtmlTargetElement(Attributes = "asp-condition")] // only apply to elements with this attribute
public class ConditionTagHelper : TagHelper
{
    [HtmlAttributeName("asp-condition")]
    public bool Condition { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!Condition)
        {
            output.SuppressOutput();
        }
    }
}


