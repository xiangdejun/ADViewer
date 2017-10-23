using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ADViewer;

namespace ADViewer_Test
{
    [TestFixture]
    public class ADHelperTests
    {
        [TestCase("", InputTypes.ID,  "")]
        [TestCase(null, InputTypes.ID, "")]
        [TestCase("28852353", InputTypes.ID, "(SAMAccountName=28852353)")]
        [TestCase("XP012807", InputTypes.ID, "(SAMAccountName=XP012807)")]
        [TestCase("darren2.xiang@sonymobile.com", InputTypes.Email, "(mail=darren2.xiang@sonymobile.com)")]
        [TestCase("xiang, darren 2", InputTypes.Name, "(cn=xiang, darren 2)")]
        public void GetSearchFilter_InputValue_ReturnFilter(string input, InputTypes inputType, string expected)
        {
            var helper = new ADHelper();
            Assert.AreEqual(expected, helper.GetSearchFilter(inputType, input));
        }

        [TestCase(InputTypes.ID, "", null)]
        [TestCase(InputTypes.ID, null, null)]
        [TestCase(InputTypes.ID, null, null)]
        public void GetPropertyModels_InputValue_ReturnFilter(InputTypes inputType, string input, List<ADModel> expect)
        {
            var helper = new ADHelper();
            Assert.AreSame(expect, helper.GetPropertyModels(inputType, input));
        }
    }
}
