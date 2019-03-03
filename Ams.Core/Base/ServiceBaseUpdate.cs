
namespace Ams.Core
{
    public partial class ServiceBase<T> where T : ModelBase, new()
    {
        protected virtual bool OnBeforeUpdate(UpdateEventArgs arg)
        {
            return true;
        }

        protected virtual void OnAfterUpdate(UpdateEventArgs arg)
        {

        }

        public int Update(ParamUpdate param)
        {
            var result = 0;
            Logger("更新记录", () =>
            {
                db.UseTransaction(true);
                var rtnBefore = this.OnBeforeUpdate(new UpdateEventArgs() { db = db, data = param.GetData() });
                if (!rtnBefore) return;
                result = BuilderParse(param).Execute();
                Msg.Set(MsgType.Success, APP.MSG_UPDATE_SUCCESS);
                this.OnAfterUpdate(new UpdateEventArgs() { db = db, data = param.GetData(), executeValue=result });
                db.Commit();
            });
            return result;
        }
    }
}
