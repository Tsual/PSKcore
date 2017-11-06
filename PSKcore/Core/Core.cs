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
        public static async Task mainAsync()
        {
            using (var _IPSKcore = Core.CoreGenerator.Current.CreateServiceObject())
            {
                var user = _IPSKcore.CreateUser("test", "test");
                await user.TryLoginAsync(LoginUser.UserNotFoundReceipt.Create, _IPSKcore);
                var cuser = _IPSKcore.CurrentUser;
                var _info = new Info(_IPSKcore) { Detail = "Detail", DetailName = "DetailName", Record = new Recording() { key = "key", uid = cuser.UID, value = "value" } };
                cuser.Recordings.Add(_info);
                _info = new Info(_IPSKcore) { Detail = "Detail", DetailName = "DetailName", Record = new Recording() { key = "key", uid = cuser.UID, value = "value" } };
                cuser.Recordings.Add(_info);
                _info = new Info(_IPSKcore) { Detail = "Detail", DetailName = "DetailName", Record = new Recording() { key = "key", uid = cuser.UID, value = "value" } };
                cuser.Recordings.Add(_info);
                _info = new Info(_IPSKcore) { Detail = "Detail", DetailName = "DetailName", Record = new Recording() { key = "key", uid = cuser.UID, value = "value" } };
                cuser.Recordings.Add(_info);
                int a = 0;

            }







        }

    }

    public class Core : IPSKcore, IDisposable
    {
        private delegate void DisposeDel(IPSKcore obj);
        private event DisposeDel Disposed;

        private Core()
        {

        }

        public CurrentUser CurrentUser { get; private set; }

        public bool isRegisted => CurrentUser == null ? false : true;

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

        public void Dispose() => Disposed?.Invoke(this);
        
        public void Regist(CurrentUser user) => CurrentUser = user ?? throw new NullReferenceException();

        public void Unsubscribe() => CurrentUser = null;

        public LoginUser CreateUser(string pid, string pwd) => LoginUser.CreateObj(pid, pwd);

        public class CoreGenerator
        {
            static CoreGenerator innerobj = null;
            public static CoreGenerator Current { get { if (innerobj == null) innerobj = new CoreGenerator(); return innerobj; } }


            private CoreGenerator() { }
            List<IPSKcore> _ServiceCollection = new List<IPSKcore>();
            public IReadOnlyList<IPSKcore> ServiceCollection { get => _ServiceCollection; }

            public Core CreateServiceObject()
            {
                Core obj = new Core();
                _ServiceCollection.Add(obj);
                obj.Disposed += Obj_Disposed;
                return obj;
            }

            private void Obj_Disposed(IPSKcore obj)
            {
                _ServiceCollection.Remove(obj);
            }
        }
    }


}
