using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

namespace DigitalPlatform.Errors.Models.Models
{
    public class CustomError
    {
        public IConfiguration Configuration { get; }
        public GlobalErrorCategory ErrorCategory { get; }
        public string AppErrorCode { get; }
        public bool IncludeErrorDescription { get; }

        public CustomError(
            IConfiguration configuration,
            GlobalErrorCategory errorCategory,
            string appErrorCode,
            bool includeErrorDescription)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ErrorCategory = errorCategory;
            AppErrorCode = appErrorCode ?? throw new ArgumentNullException(nameof(appErrorCode));
            IncludeErrorDescription = includeErrorDescription;
        }
    }
}
