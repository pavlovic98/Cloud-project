#pragma checksum "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "837d4e2da0c8b68c2094d0f18f27c7b0b9e983c9"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Active), @"mvc.1.0.view", @"/Views/Home/Active.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Active.cshtml", typeof(AspNetCore.Views_Home_Active))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\_ViewImports.cshtml"
using ExamRegistration.Web;

#line default
#line hidden
#line 2 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\_ViewImports.cshtml"
using ExamRegistration.Web.Models;

#line default
#line hidden
#line 1 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
using Db.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"837d4e2da0c8b68c2094d0f18f27c7b0b9e983c9", @"/Views/Home/Active.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7ab3a967edd91fdd8388af9f43e634bbe0877906", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Active : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Exam>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(45, 299, true);
            WriteLiteral(@"
<h1>ACTIVE EXAMS</h1>

<table class=""table table-hover table-striped table-dark"">
    <tr>
        <th>First name</th>
        <th>Last name</th>
        <th>Index</th>
        <th>Exam name</th>
        <th>Profesor name</th>
        <th>Date</th>
        <th>Period</th>

    </tr>
");
            EndContext();
#line 17 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
     foreach (var item in Model)
    {

#line default
#line hidden
            BeginContext(385, 30, true);
            WriteLiteral("        <tr>\r\n            <td>");
            EndContext();
            BeginContext(416, 14, false);
#line 20 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.FirstName);

#line default
#line hidden
            EndContext();
            BeginContext(430, 23, true);
            WriteLiteral("</td>\r\n            <td>");
            EndContext();
            BeginContext(454, 13, false);
#line 21 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.LastName);

#line default
#line hidden
            EndContext();
            BeginContext(467, 23, true);
            WriteLiteral("</td>\r\n            <td>");
            EndContext();
            BeginContext(491, 10, false);
#line 22 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.Index);

#line default
#line hidden
            EndContext();
            BeginContext(501, 23, true);
            WriteLiteral("</td>\r\n            <td>");
            EndContext();
            BeginContext(525, 13, false);
#line 23 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.ExamName);

#line default
#line hidden
            EndContext();
            BeginContext(538, 23, true);
            WriteLiteral("</td>\r\n            <td>");
            EndContext();
            BeginContext(562, 17, false);
#line 24 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.ProfesorName);

#line default
#line hidden
            EndContext();
            BeginContext(579, 23, true);
            WriteLiteral("</td>\r\n            <td>");
            EndContext();
            BeginContext(603, 9, false);
#line 25 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.Date);

#line default
#line hidden
            EndContext();
            BeginContext(612, 6, true);
            WriteLiteral("&nbsp;");
            EndContext();
            BeginContext(619, 9, false);
#line 25 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
                           Write(item.Time);

#line default
#line hidden
            EndContext();
            BeginContext(628, 23, true);
            WriteLiteral("</td>\r\n            <td>");
            EndContext();
            BeginContext(652, 15, false);
#line 26 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
           Write(item.ExamPeriod);

#line default
#line hidden
            EndContext();
            BeginContext(667, 22, true);
            WriteLiteral("</td>\r\n        </tr>\r\n");
            EndContext();
#line 28 "C:\Users\Mladen\Desktop\ExamRegistration\ExamRegistration.Web\Views\Home\Active.cshtml"
    }

#line default
#line hidden
            BeginContext(696, 8, true);
            WriteLiteral("</table>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Exam>> Html { get; private set; }
    }
}
#pragma warning restore 1591
