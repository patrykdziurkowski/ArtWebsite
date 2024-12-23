using System;

namespace tests.e2e.utils;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestPriorityAttribute : Attribute
{
        public TestPriorityAttribute(int priority)
        {
                Priority = priority;
        }

        public int Priority { get; private set; }
}
