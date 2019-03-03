using System.Web.Http;

namespace hippos_api
{
    /// <summary>
    /// API基类
    /// </summary>
    public abstract class ControllerBase : ApiController
    {
        #region 返回 ，成功带数据，错误,异常 默认方法
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual IHttpActionResult Success(string message)
        {
            return Content(System.Net.HttpStatusCode.OK, new AjaxResult { state = ResultType.success.ToString(), message = message });
        }
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual IHttpActionResult Success(string message, object data)
        {
            return Content(System.Net.HttpStatusCode.OK, new AjaxResult { state = ResultType.success.ToString(), message = message, data = data });
        }
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual IHttpActionResult Error(string message)
        {
            return Content(System.Net.HttpStatusCode.OK, new AjaxResult { state = ResultType.error.ToString(), message = message });
        }
        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual IHttpActionResult Exception(string message)
        {
            return Content(System.Net.HttpStatusCode.OK, new AjaxResult { state = ResultType.exception.ToString(), message = message });
        }
        #endregion
    }
}
