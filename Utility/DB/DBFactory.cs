using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Objects;

namespace Utility.DB
{
    public static class DBFactory
    {
        public static ADO CreateMSADO(string conn) 
        {
            return new ADO(conn, new MsSql());
        }

        public static EF<T> CreateMSEF<T>() where T : ObjectContext,  new()
        {
            return new EF<T>(new MsSql());
        }
    }

    /* How to use
        public class AdvQuery : DBQuery
        {
            [ResultSetAttrubue(0)]
            public List<BorrowerModel> Borrowers { get; set; }

            [ResultSetAttrubue(1)]
            public List<BorrowerIncome> Incomes { get; set; }

            [ResultSetAttrubue(2)]
            public List<BorrowerLiability> Liabilities { get; set; }
        }

        public static void Test()
        {
            var db = DBFactory.CreateMSADO("Data Source=10.10.50.107; Initial Catalog=EALOANS; user id=ealoans_writer;password=E@dBWr1t3r;");
            var query = new AdvQuery { CommandText = "pGetBorrowerInfo", IsSql = false, Inputs = new { loanNumber = "1714070635" } };
            db.Advanced(query);
        }
     */
}
