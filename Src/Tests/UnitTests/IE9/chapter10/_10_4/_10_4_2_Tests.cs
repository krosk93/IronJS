// <auto-generated />
namespace IronJS.Tests.UnitTests.IE9.chapter10._10_4
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class _10_4_2_Tests : IE9TestFixture
    {
        public _10_4_2_Tests() : base(@"chapter10\10.4\10.4.2") { }

        [Test(Description = "Indirect call to eval has context set to global context")] public void _10_4_2__1__1() { RunFile(@"10.4.2-1-1.js"); }
        [Test(Description = "Indirect call to eval has context set to global context (nested function)")] public void _10_4_2__1__2() { RunFile(@"10.4.2-1-2.js"); }
        [Test(Description = "Indirect call to eval has context set to global context (catch block)")] public void _10_4_2__1__3() { RunFile(@"10.4.2-1-3.js"); }
        [Test(Description = "Indirect call to eval has context set to global context (with block)")] public void _10_4_2__1__4() { RunFile(@"10.4.2-1-4.js"); }
        [Test(Description = "Indirect call to eval has context set to global context (inside another eval)")] public void _10_4_2__1__5() { RunFile(@"10.4.2-1-5.js"); }
        [Test(Description = "Direct val code in non-strict mode - can instantiate variable in calling context")] public void _10_4_2__2__c__1() { RunFile(@"10.4.2-2-c-1.js"); }
    }
}