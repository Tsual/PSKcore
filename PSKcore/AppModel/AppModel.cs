using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PSKcore.AppModel
{

    //public class ATT_INFO_Str
    //{
    //    public string str { get; set; }
    //}

    //public class ATT_INFO
    //{


    //    public Info _Info { get; set; }
    //    public ObservableCollection<ATT_INFO_Str> Lines { get => _Lines; }
    //    private ObservableCollection<ATT_INFO_Str> _Lines = new ObservableCollection<ATT_INFO_Str>();
    //    private int _infoindex = 0;


    //    public ATT_INFO(Info info)
    //    {
    //        if (info != null)
    //            _Info = info;
    //        else
    //            throw new NullReferenceException();
    //        _infoindex = Core.Current.CurrentUser.Recordings.IndexOf(info);
    //        _deserialize();
    //        _Lines.CollectionChanged += _Lines_CollectionChanged;
    //    }

    //    public void _Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    //    {
    //        _serializeAsync();
    //    }

    //    private void _serializeAsync()
    //    {
    //        try
    //        {
    //            using (MemoryStream ms = new MemoryStream())
    //            {
    //                DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<ATT_INFO_Str>));
    //                ser.WriteObject(ms, _Lines);
    //                var res = Encoding.UTF8.GetString(ms.ToArray());
    //                Core.Current.CurrentUser.Recordings[_infoindex] = new Info() { DetailName = _Info.DetailName, Record = _Info.Record, Detail = res };
    //            }
    //        }
    //        finally { }
    //    }

    //    private void _deserialize()
    //    {
    //        if (_Info.Detail == "") return;
    //        try
    //        {
    //            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_Info.Detail)))
    //            {
    //                DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<ATT_INFO_Str>));
    //                if (ser.ReadObject(ms) is ObservableCollection<ATT_INFO_Str> res)
    //                {
    //                    var t = res;
    //                    _Lines = res;
    //                }

    //            }
    //        }
    //        finally { }


    //    }
    //}


    //public class DataPac
    //{
    //    public class dRow
    //    {
    //        public string key { get; set; }
    //        public string value { get; set; }
    //    }


    //    public dRow[] dRows { get; set; }
    //    public string token { get; set; }
    //    public string sertoken { get; set; }
    //    public string pid { get; set; }
    //    public string pwd_hash { get; set; }
    //    public string vertifystr { get; set; }

    //    public async Task SerializeAsync(StorageFile sf)
    //    {
    //        using (IRandomAccessStream rastream = await sf.OpenAsync(FileAccessMode.ReadWrite))
    //        {
    //            using (IOutputStream os = rastream.GetOutputStreamAt(0))
    //            {
    //                DataContractSerializer ser = new DataContractSerializer(typeof(DataPac));
    //                ser.WriteObject(os.AsStreamForWrite(), this);
    //                await os.FlushAsync();
    //            }
    //        }
    //    }

    //    public static async Task<DataPac> DeserializeAsync(StorageFile sf)
    //    {
    //        using (IRandomAccessStream rastream = await sf.OpenAsync(FileAccessMode.ReadWrite))
    //        {
    //            using (IInputStream istraem = rastream.GetInputStreamAt(0))
    //            {
    //                DataContractSerializer ser = new DataContractSerializer(typeof(DataPac));
    //                var datpac = ser.ReadObject(istraem.AsStreamForRead()) as DataPac;
    //                return datpac;
    //            }
    //        }
    //    }






    //}

    //public class DataPacManager
    //{

    //    //useage
    //    //var spicker = new FileSavePicker();
    //    //spicker.FileTypeChoices.Add("XML", new List<String>() { ".xml" });
    //    //var sfs = await spicker.PickSaveFileAsync();
    //    //await DataPacManager.SerializeAsync(sfs, "test");
    //    public static async Task SerializeAsync(StorageFile sf, string serkey)
    //    {
    //        if (Core.Current.CurrentUser == null) throw new Exception();
    //        DataPac dat = new DataPac();
    //        await Task.Run(async () =>
    //        {
    //            //create ser aesobj
    //            string pid = Core.Current.CurrentUser.PID;
    //            dat.pid = pid;
    //            string str1 = pid + serkey;
    //            string str2 = serkey + pid;
    //            var hashobj = new Helper.HashProvider();
    //            string hstr1 = hashobj.Hash(str1);
    //            string hstr2 = hashobj.Hash(str2);
    //            string serpwd_hash = hstr1 + hstr2;
    //            Helper.AESProvider _AESobj;
    //            var ivhash = new Helper.HashProvider(HashAlgorithmNames.Md5);
    //            byte[] _iv = ivhash.Hashbytes(pid);
    //            //create token
    //            string ranstr = (new Helper.RandomGenerator()).getRandomString(20);
    //            dat.token = ranstr;
    //            string kstr1 = ranstr + serpwd_hash;
    //            string kstr2 = serpwd_hash + ranstr;

    //            var keyhash = new Helper.HashProvider();
    //            byte[] _key = new byte[128];
    //            byte[] btar = keyhash.Hashbytes(kstr1);
    //            Array.Copy(btar, 0, _key, 0, 64);
    //            btar = keyhash.Hashbytes(kstr2);
    //            Array.Copy(btar, 0, _key, 64, 64);
    //            _AESobj = new Helper.AESProvider(_iv, _key);
    //            //encrypt dats
    //            dat.pwd_hash = _AESobj.Encrypt(Core.Current.CurrentUser.PWD_hash);
    //            dat.sertoken = _AESobj.Encrypt(ranstr);
    //            dat.vertifystr = _AESobj.Encrypt(AssetsController.getLocalSequenceString(Core.Current.CurrentUser.UID));
    //            List<DataPac.dRow> arr = new List<DataPac.dRow>();
    //            using (APPDbContext db = new APPDbContext())
    //            {
    //                foreach (var t in db.Recordings.ToList())
    //                {
    //                    if (t.uid == Core.Current.CurrentUser.UID)
    //                    {
    //                        DataPac.dRow obj = new DataPac.dRow()
    //                        {
    //                            key = _AESobj.Encrypt(t.key),
    //                            value = _AESobj.Encrypt(t.value)
    //                        };
    //                        arr.Add(obj);
    //                    }
    //                }
    //            }
    //            dat.dRows = arr.ToArray();
    //            await dat.SerializeAsync(sf);
    //        });
    //    }

    //    //useage
    //    //var picker = new FileOpenPicker();
    //    //picker.FileTypeFilter.Add(".xml");
    //    //    var sf = await picker.PickSingleFileAsync();
    //    //await DataPacManager.DeserializeAsync(sf, "test");@throw KeyVertifyFailException
    //    public static async Task DeserializeAsync(StorageFile sf, string serkey)
    //    {
    //        var dat = await DataPac.DeserializeAsync(sf);
    //        string pid = dat.pid;
    //        string token = dat.token;
    //        string desertoken = "";
    //        Helper.AESProvider _AESobj = null;
    //        await Task.Run(() =>
    //        {
    //            //create ser aesobj
    //            string str1 = pid + serkey;
    //            string str2 = serkey + pid;
    //            var hashobj = new Helper.HashProvider();
    //            string hstr1 = hashobj.Hash(str1);
    //            string hstr2 = hashobj.Hash(str2);
    //            string serpwd_hash = hstr1 + hstr2;
    //            var ivhash = new Helper.HashProvider(HashAlgorithmNames.Md5);
    //            byte[] _iv = ivhash.Hashbytes(pid);

    //            string kstr1 = token + serpwd_hash;
    //            string kstr2 = serpwd_hash + token;

    //            var keyhash = new Helper.HashProvider();
    //            byte[] _key = new byte[128];
    //            byte[] btar = keyhash.Hashbytes(kstr1);
    //            Array.Copy(btar, 0, _key, 0, 64);
    //            btar = keyhash.Hashbytes(kstr2);
    //            Array.Copy(btar, 0, _key, 64, 64);
    //            _AESobj = new Helper.AESProvider(_iv, _key);
    //            desertoken = _AESobj.Decrypt(dat.sertoken);
    //        });
    //        if (desertoken != token) throw new KeyVertifyFailException() { _DataPac = dat };
    //        await Task.Run(() =>            //InvalidOperationException
    //        {
    //            using (APPDbContext db = new APPDbContext())
    //            {
    //                //search user
    //                User user = null;
    //                foreach (var t in db.Users.ToList())
    //                {
    //                    if (t.pid == pid)
    //                        user = t;
    //                }

    //                if (user == null)
    //                {
    //                    string pwd_hash_aes = AssetsController.EncryptwithAppaesobj(_AESobj.Decrypt(dat.pwd_hash));
    //                    User dbuser = new User()
    //                    {
    //                        pid = pid,
    //                        pwd = pwd_hash_aes
    //                    };
    //                    db.Entry(dbuser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
    //                    db.SaveChanges();
    //                    user = db.Users.Single(b => b.pid == pid); ;
    //                }
    //                //set user sa
    //                AssetsController.getLocalSequenceString(user.ID);
    //                var saobj = db.SA.Single(t => t.ID == user.ID);
    //                //saobj.Data = dat.vertifystr;
    //                saobj.Data = _AESobj.Decrypt(dat.vertifystr);
    //                db.Entry(saobj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

    //                foreach (var t in dat.dRows)
    //                {
    //                    Recording obj = new Recording()
    //                    {
    //                        uid = user.ID,
    //                        key = _AESobj.Decrypt(t.key),
    //                        value = _AESobj.Decrypt(t.value)
    //                    };
    //                    db.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Added;
    //                }
    //                db.SaveChanges();
    //            }
    //        });
    //    }

    //    public class KeyVertifyFailException : Exception
    //    {
    //        public override string Message => "decode sertoken cannot match token";
    //        public DataPac _DataPac { get; set; }
    //    }


    //}
}
