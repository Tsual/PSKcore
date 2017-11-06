using PSK.Helper;
using PSKcore.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;

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
            _appiv = new byte[16];
            new Random().NextBytes(_appiv);
            return _appiv;
        }

        static byte[] decodeappkey(string keystr)
        {
            var slist = keystr.Split('|');
            if (slist.Length != 32) throw new Exception("iv dec error");
            byte[] res = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                res[i] = Convert.ToByte(slist[i]);
            }
            return res;
        }
        static string encodeappkey(byte[] key)
        {
            if (key == null || key.Length != 32) throw new Exception("key error");
            string res = "" + key[0];
            for (int i = 1; i < 32; i++)
            {
                res += "|" + key[i];
            }
            return res;
        }
        static byte[] createappkey()
        {
            _appkey = new byte[32];
            new Random().NextBytes(_appkey);
            return _appkey;
        }

        public static bool Reset()
        {

            //LocalSettingReader.Current.Settings.Remove(LocalSettingReader.Current[STR_FS]?.ElementAt(0));
            //LocalSettingReader.Current.Settings.Remove(LocalSettingReader.Current[STR_APP_IV]?.ElementAt(0));
            //LocalSettingReader.Current.Settings.Remove(LocalSettingReader.Current[STR_APP_KEY]?.ElementAt(0));

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

        public static string getRecordingSequenceString(int id)
        {
            using (APPDbContext db = new APPDbContext())
            {
                var res = db.SB.Find(id);
                if (res != null)
                    return res.Data;
                else
                {
                    var ncount = db.SB.Count();
                    var rans = new RandomGenerator();
                    for (int i = 0; i < id - ncount; i++)
                    {
                        db.SB.Add(new StringSequenceObjB() { Data = rans.getRandomString(20) });
                    }
                    db.SaveChanges();
                    res = db.SB.Find(id);
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
                _appiv = decodeappiv(LocalSettingReader.Current[STR_APP_IV]?.Value);
                _appkey = decodeappkey(LocalSettingReader.Current[STR_APP_KEY]?.Value);
            }
            else
            {
                LocalSettingReader.Current.Settings.Add(new LocalSetting() { Key = STR_FS, Value = "False" });
                LocalSettingReader.Current.Settings.Add(new LocalSetting() { Key = STR_APP_IV, Value = encodeappiv(createappiv()) });
                LocalSettingReader.Current.Settings.Add(new LocalSetting() { Key = STR_APP_KEY, Value = encodeappkey(createappkey()) });
            }
            
        }

        private AssetsController()
        {

        }








    }


}
