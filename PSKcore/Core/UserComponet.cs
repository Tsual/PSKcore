using PSK.Helper;
using PSKcore.AppModel;
using PSKcore.DbModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace PSKcore
{
    public class CurrentUser
    {
        internal CurrentUser(List<Recording> list, string PID, string PWD_hash, int UID)
        {
            this.PID = PID;
            this.PWD_hash = PWD_hash;
            this.UID = UID;

            //for (int i = 0; i < list.Count; i++)
            //    recordings.Add(new Info(list[i], this));
            foreach (var t in list)
                recordings.Add(new Info(t, this));
            recordings.CollectionChanged += Recordings_CollectionChanged;
        }

        private void Recordings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            using (APPDbContext db = new APPDbContext())
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var t in e.NewItems)
                        {
                            db.Entry(((Info)t).Encode(this)).State =
                                Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var t in e.OldItems)
                        {
                            db.Entry(((Info)t).Record).State =
                                Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (var t in e.NewItems)
                        {
                            db.Entry(((Info)t).Modify(this)).State =
                                Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        foreach (var t in e.OldItems)
                        {
                            db.Entry(((Info)t).Record).State =
                                Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        }
                        foreach (var t in e.NewItems)
                        {
                            db.Entry(((Info)t).Encode(this)).State =
                                Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                        break;
                }
                db.SaveChanges();
            }
        }

        public string PID { get; private set; }
        public string PWD_hash { get; private set; }
        public int UID { get; private set; }
        private AESProvider AESobj
        {
            get
            {
                if (_AESobj == null)
                {
                    var ivhash = new HashProvider(HashAlgorithmNames.Md5);
                    byte[] _iv = ivhash.Hashbytes(PID);

                    string ranstr = AssetsController.getLocalSequenceString(UID);
                    string kstr1 = ranstr + PWD_hash;
                    string kstr2 = PWD_hash + ranstr;

                    var keyhash = new HashProvider();
                    byte[] _key = new byte[128];
                    byte[] btar = keyhash.Hashbytes(kstr1);
                    Array.Copy(btar, 0, _key, 0, 64);
                    btar = keyhash.Hashbytes(kstr2);
                    Array.Copy(btar, 0, _key, 64, 64);
                    _AESobj = new AESProvider(_iv, _key);

                }


                return _AESobj;
            }
        }
        AESProvider _AESobj;

        public ObservableCollection<Info> Recordings { get => recordings; }
        private ObservableCollection<Info> recordings = new ObservableCollection<Info>();

        public string Decode(string metaStr)
        {
            return AESobj.Decrypt(metaStr);
        }
        public string Encode(string metaStr)
        {
            return AESobj.Encrypt(metaStr);
        }
        public void Logout()
        {

            Core.Current.Unsubscribe();
            UserUnsubscribedEvent?.Invoke(this);
        }

        public delegate void UserUnsubscribedEventHandler(CurrentUser user);
        public event UserUnsubscribedEventHandler UserUnsubscribedEvent;

    }
}
