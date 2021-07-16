using System.Net;

namespace UserCenter.Common.Response
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T>
    {
        public HttpStatusCode Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public static BaseResponse<T> Error(string msg = "", HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError, T data = default(T))
        {
            return new BaseResponse<T>()
            {
                Code = httpStatusCode,
                Msg = msg,
                Data = data
            };
        }

        public static BaseResponse<T> Success(T data = default(T))
        {
            return new BaseResponse<T>()
            {
                Code = HttpStatusCode.OK,
                Msg = "成功",
                Data = data
            };
        }
    }
}
