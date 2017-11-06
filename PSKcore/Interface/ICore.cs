using PSKcore.AppModel;

namespace PSKcore.Interface
{
    public interface IPSKcore
    {
        CurrentUser CurrentUser { get; }
        bool isRegisted { get; }

        void DeleteUser();
        void Regist(CurrentUser user);
        void Unsubscribe();
        LoginUser CreateUser(string pid, string pwd);
    }
}