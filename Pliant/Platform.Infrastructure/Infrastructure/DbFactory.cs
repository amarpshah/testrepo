using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.BizComponent.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        ExamContext dbContext;

        public ExamContext Init()
        {
            return dbContext ?? (dbContext = new ExamContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
