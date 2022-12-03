using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvDebug
{
    public class Assertion
    {
        private Assertion() { }
        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="condition">条件为false执行</param>
        /// <param name="message"></param>
        public static void Assert(bool condition, string message)
        {
            System.Diagnostics.Debug.Assert(condition, message);
        }
    }
}
