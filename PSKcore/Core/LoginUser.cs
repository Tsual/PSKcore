using PSK.Helper;
using PSKcore.AppModel;
using PSKcore.DbModel;
using PSKcore.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace PSKcore
{
    public class LoginUser
    {
        public string PID { get;private set; }
        public string PWD_hash { get;private set; }

        private LoginUser() { }

        internal static LoginUser CreateObj(string pid, string pwd)
        {
            var user = new LoginUser();
            user.PID = pid;

            //加盐方式 str1=pid+pwd str2=pwd+pid
            string str1 = pid + pwd;
            string str2 = pwd + pid;
            var hashobj = new HashProvider();
            string hstr1 = hashobj.Hash(str1);
            string hstr2 = hashobj.Hash(str2);
            user.PWD_hash = hstr1 + hstr2;

            return user;
        }

        public async Task TryLoginAsync(UserNotFoundReceipt e, IPSKcore service)
        {

            using (APPDbContext db = new APPDbContext())
            {
                bool b_UserVertifyEvent = false;
                bool b_UserPwdVertifyFailEvent = false;
                bool b_UserNotFoundEvent = false;

                await Task.Run(() =>
                {


                    string pwd_hash_aes = AssetsController.EncryptwithAppaesobj(PWD_hash);

                    var iuserlist = from t in db.Users
                                    where t.pid == PID
                                    select t;
                    if (iuserlist.Count() == 0)
                    {
                        switch (e)
                        {
                            case UserNotFoundReceipt.Create:
                                User dbuser = new User()
                                {
                                    pid = PID,
                                    pwd = pwd_hash_aes
                                };
                                db.Entry(dbuser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                                db.SaveChangesAsync();
                                break;
                            case UserNotFoundReceipt.None:
                                b_UserNotFoundEvent = true;
                                return;
                        }
                    }
                    else
                    {
                        var vertifyuser = iuserlist.ToList()[0];
                        if (vertifyuser.pid != PID || vertifyuser.pwd != pwd_hash_aes)
                        {
                            b_UserPwdVertifyFailEvent = true;
                        }
                    }
                    int UID = (from t in db.Users
                               where PID == t.pid && pwd_hash_aes == t.pwd
                               select t).ToList()[0].ID;

                    var rlist = from t in db.Recordings
                                 where t.uid == UID
                                select t;

                    service.Regist(new CurrentUser(rlist, PID, PWD_hash, UID, service));
                    b_UserVertifyEvent = true;
                });



                if (b_UserPwdVertifyFailEvent)
                    UserPwdVertifyFailEvent?.Invoke(this);
                if (b_UserVertifyEvent)
                    UserVertifyEvent?.Invoke(this);
                if (b_UserNotFoundEvent)
                    UserNotFoundEvent?.Invoke(this);
            }
        }


        public delegate void UserVertifyEventHandler(LoginUser user);
        public event UserVertifyEventHandler UserVertifyEvent;

        public delegate void UserPwdVertifyFailEventHandler(LoginUser user);
        public event UserPwdVertifyFailEventHandler UserPwdVertifyFailEvent;

        //public delegate void UserCreatePidExistEventHandler(LoginUser user);
        //public event UserCreatePidExistEventHandler UserCreatePidExistEvent;

        public enum UserNotFoundReceipt { Create, None }
        public delegate void UserNotFoundEventHandler(LoginUser user);
        public event UserNotFoundEventHandler UserNotFoundEvent;

    }
}
