using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalPlatform.Errors.Models.Models
{
    public enum GlobalErrorCategory
    {
        [Description("GEN-1000")]
        General,

        [Description("AUTH-1000")]
        Authentication,

        [Description("AUTH-2000")]
        Authorization,

        [Description("VAL-1000")]
        Validation,

        [Description("DB-1000")]
        Database,

        [Description("NET-1000")]
        Network,

        [Description("CLIENT-1000")]
        ClientSide,

        [Description("SERVER-1000")]
        ServerSide,

        [Description("API-1000")]
        API,

        [Description("SEC-1000")]
        Security,

        [Description("CONFIG-1000")]
        Configuration,

        [Description("DEPEN-1000")]
        Dependency,

        [Description("UNHAND-1000")]
        Unhandled,
        Business
    }

}
