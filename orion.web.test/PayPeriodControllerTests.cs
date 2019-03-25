using Microsoft.VisualStudio.TestTools.UnitTesting;
using orion.web.Common;
using orion.web.Controllers;
using orion.web.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace orion.web.test
{
    [TestClass]
    public class PayPeriodControllerTests
    {
        [TestMethod]
        public void ByDefaultGet30PP_With2InTheFuture()
        {
            var ut = new PayPeriodController();
            var res = ut.Get();
            
            res.
        }
    }
}
