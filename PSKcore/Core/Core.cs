using PSKcore.AppModel;
using PSKcore.DbModel;
using PSKcore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSKcore
{
    public class Test
    {
        public static void main()
        {


            int a = 0;
        }

    }

    public class Core : ICore
    {
        static Core innerobj;
        static Core()
        {

        }

        public static ICore Current
        {
            get
            {
                if (innerobj == null)
                    innerobj = new Core();
                return innerobj;
            }
        }
        private Core()
        {

        }

        public CurrentUser CurrentUser { get; private set; }

        public bool isRegisted => innerobj == null ? false : true;

        public ATT_INFO Databridge { get; set; }

        public void DeleteUser()
        {
            using (APPDbContext db = new APPDbContext())
            {
                foreach (var t in CurrentUser.Recordings)
                {
                    db.Entry(t.Record).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                db.Entry(db.Users.Single(b => b.ID == CurrentUser.UID)).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public void Regist(CurrentUser user) => CurrentUser = user ?? throw new NullReferenceException();

        public void Unsubscribe() => CurrentUser = null;


    }

    public class HashAlgorithmNames
    {
        public const string Md5 = "MD5";
    }
}
