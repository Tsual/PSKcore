using PSK.Helper;
using PSKcore.DbModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PSKcore.AppModel
{
    public class AssetsController
    {
        const string STR_FS = "firstinstall";
        const string STR_APP_IV = "appiv";
        const string STR_APP_KEY = "appkey";

        private static byte[] _appiv;
        private static byte[] _appkey;

        static byte[] decodeappiv(string ivstr)
        {
            var slist = ivstr.Split('|');
            if (slist.Length != 16) throw new Exception("iv dec error");
            byte[] res = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                res[i] = Convert.ToByte(slist[i]);
            }
            return res;

        }
        static string encodeappiv(byte[] iv)
        {
            if (iv == null || iv.Length != 16) throw new Exception("iv error");
            string res = "" + iv[0];
            for (int i = 1; i < 16; i++)
            {
                res += "|" + iv[i];
            }
            return res;
        }
        static byte[] createappiv()
        {
            byte[] res = new byte[16];
            new Random().NextBytes(res);
            return res;
        }

        static byte[] decodeappkey(string keystr)
        {
            var slist = keystr.Split('|');
            if (slist.Length != 128) throw new Exception("iv dec error");
            byte[] res = new byte[128];
            for (int i = 0; i < 128; i++)
            {
                res[i] = Convert.ToByte(slist[i]);
            }
            return res;
        }
        static string encodeappkey(byte[] key)
        {
            if (key == null || key.Length != 128) throw new Exception("key error");
            string res = "" + key[0];
            for (int i = 1; i < 128; i++)
            {
                res += "|" + key[i];
            }
            return res;
        }
        static byte[] createappkey()
        {
            byte[] res = new byte[128];
            new Random().NextBytes(res);
            return res;
        }

        public static bool Reset()
        {
            try
            {
                LocalSettingReader.Current.Settings.Remove(LocalSettingReader.Current[STR_FS]?.ElementAt(0));
                LocalSettingReader.Current.Settings.Remove(LocalSettingReader.Current[STR_APP_IV]?.ElementAt(0));
                LocalSettingReader.Current.Settings.Remove(LocalSettingReader.Current[STR_APP_KEY]?.ElementAt(0));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string EncryptwithAppaesobj(string metaStr)
        {
            var aesobj = new AESProvider(_appiv, _appkey);
            return aesobj.Encrypt(metaStr);
        }

        public static string getLocalSequenceString(int id)
        {
            using (APPDbContext db = new APPDbContext())
            {
                var res = db.SA.Find(id);
                if (res != null)
                    return res.Data;
                else
                {
                    var ncount = db.SA.Count();
                    var rans = new RandomGenerator();
                    for (int i = 0; i < id - ncount; i++)
                    {
                        db.SA.Add(new StringSequenceObjA() { Data = rans.getRandomString(20) });
                    }
                    db.SaveChanges();
                    res = db.SA.Find(id);
                    if (res == null) throw new Exception("sa id miss");
                    return res.Data;
                }
            }
        }

        static AssetsController()
        {
            //init appiv appkey
            if (LocalSettingReader.Current.ContainsKey(STR_FS))
            {
                _appiv = decodeappiv(LocalSettingReader.Current[STR_APP_IV]?.ElementAt(0).Value);
                _appkey = decodeappkey(LocalSettingReader.Current[STR_APP_KEY]?.ElementAt(0).Value);
            }
            else
            {
                LocalSettingReader.Current[STR_FS].ElementAt(0).Value = "false";
                _appiv = createappiv();
                LocalSettingReader.Current[STR_APP_IV].ElementAt(0).Value= encodeappiv(_appiv);
                _appkey = createappkey();
                LocalSettingReader.Current[STR_APP_KEY].ElementAt(0).Value = encodeappkey(_appkey);
            }
            
        }

        private AssetsController()
        {

        }

        public static void test()
        {

            //delete all records in users
            //using (APPDbContext db = new APPDbContext())
            //{
            //    db.Database.EnsureCreated();

            //    var list = (from t in db.Users.ToList()
            //                select t).ToList();
            //    foreach (var t in list)
            //    {
            //        db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            //    }
            //    db.SaveChanges();
            //}

            //using (APPDbContext db = new APPDbContext())
            //{
            //    db.Database.EnsureCreated();

            //    var list = (from t in db.Recordings.ToList()
            //                select t).ToList();
            //    foreach (var t in list)
            //    {
            //        db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            //    }
            //    db.SaveChanges();
            //}



            //var user = LoginUser.CreateObj("test", "test");
            //user.UserNotFoundEvent += (obj) => { return LoginUser.UserNotFoundReceipt.Create; };
            //var cuser = user.TryLogin();
            //var item = new Info() { Title = "ffff", Detail = "detail" };
            //cuser.Recordings.Add(item);
            //cuser.Recordings.Remove(item);














            //int a = 0;
        }






    }

    public class LocalSettingReader
    {
        private LocalSettingReader() { }

        static LocalSettingReader()
        {
            APPDbContext db = new APPDbContext();
            db.Database.EnsureCreated();
            foreach(var t in db.LocalSettings)
            {
                LocalSetting set = t;
                set.LocalSettingChanged += Set_LocalSettingChanged;
                _settings.Add(set);
            }
            _settings.CollectionChanged += _settings_CollectionChanged;



        }

        private static void _settings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            APPDbContext db = new APPDbContext();
            if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var t in e.NewItems)
                    db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            }
            else if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var t in e.OldItems)
                    db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }
            else if(e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                var _old = db.LocalSettings.Find((e.OldItems[0] as LocalSetting)?.ID);
                var _listnew = e.NewItems[0] as LocalSetting;
                _old.Key = _listnew?.Key;
                _old.Value = _listnew?.Value;
                db.Entry(_old).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            db.SaveChanges();
        }

        private static void Set_LocalSettingChanged(LocalSetting setting)
        {
            APPDbContext db = new APPDbContext();
            db.Entry(setting.dbsetting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
        }

        static ObservableCollection<LocalSetting> _settings = new ObservableCollection<LocalSetting>();
        private static LocalSettingReader _Current = new LocalSettingReader();

        public IList<LocalSetting> Settings { get => _settings; }
        public static LocalSettingReader Current { get => _Current;  }

        public IEnumerable<LocalSetting> this[string Key]
        {
            get =>from t in _settings
                       where t.Key == Key
                       select t;
        }

        public bool ContainsKey(string key)
        {
            return (from t in _settings
                    where t.Key == key
                    select t).Count() > 0;
        }

        public bool ContainsValue(string value)
        {
            return (from t in _settings
                    where t.Value == value
                    select t).Count() > 0;
        }


    }


}
