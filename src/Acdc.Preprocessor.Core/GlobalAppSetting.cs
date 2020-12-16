using Acdc.Preprocessor.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acdc.Preprocessor.Core
{
  public class GlobalAppSetting
    {
        protected GlobalAppSetting()
        {

        }
        public static AppSettings appsetting { get; set; }

    }
}
