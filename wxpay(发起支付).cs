using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


using System.Net;
using System.IO;
using System.Xml;
using Com.Tencent.MM.Sdk.Openapi;
using Com.Tencent.MM.Sdk.Modelpay;
using Com.Tencent.MM.Sdk.Modelmsg;
using System.Threading;
using Android.Support.V7.App;

namespace com.jinjianlaowu
{
    [Activity(Label = "会员企业")]
    [IntentFilter(new string[] {
        "android.intent.action.VIEW"
    }, Categories = new string[]{
        "android.intent.category.DEFAULT"
    }, DataScheme = "wx4f1353d1be138ef9")]
    public class vipcompany : activitybase
    {
        public static MyApplication am;
        public static Button btn_tovip;
        public static IWXAPI wxApi;
        public static string appid = "";
        public static string mchid = "";
        public static string AppKey = "";

        public static string orderid;
        public static TextView vipstate;
        public static AppCompatActivity satetact;
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
           
            SetContentView(Resource.Layout.vipcompany);
            am = (MyApplication)this.Application;
            orderid = DateTime.Now.ToString("yyyyMMddHHmmss") +am.Appuser.Uid;
            var toolbar = inittoolbar("会员企业");
            //注册微信
            satetact = this;
            wxApi = WXAPIFactory.CreateWXAPI(this, appid, true);
            wxApi.RegisterApp(appid);
            btn_tovip = FindViewById<Button>(Resource.Id.btn_vip);
            vipstate = FindViewById<TextView>(Resource.Id.vipstate);
          
            btn_tovip.Click += (s, e) =>
            {
                
                if (wxApi.IsWXAppInstalled)
                {
                    sendpayorder();
                }
                else
                {
                    Toast.MakeText(satetact, "该手机上未安装微信", ToastLength.Short).Show();
                }
            };

        }

      
        private static String BuildTransaction(String type)
        {
            return (type == null) ? DateTime.Now.ToLongTimeString()
                    : type + DateTime.Now.ToLongTimeString();
        }

        //发起预支付请求
        public void sendpayorder()
        {
            string TimeStamp = getTimestamp();
            //随机字符串 
            string NonceStr = getNoncestr();
            //创建支付应答对象
            var packageReqHandler = new RequestHandler();
            //初始化
            packageReqHandler.init();

            packageReqHandler.setParameter("body", "会员公司费用"); //商品信息 127字符
            packageReqHandler.setParameter("appid", appid);
            packageReqHandler.setParameter("mch_id", mchid);
            packageReqHandler.setParameter("nonce_str", NonceStr.ToLower());
            packageReqHandler.setParameter("notify_url", "http://XXX/pay.aspx");

            packageReqHandler.setParameter("out_trade_no", orderid); //商家订单号
            packageReqHandler.setParameter("spbill_create_ip", "192.168.1.1"); //用户的公网ip，不是商户服务器IP
            //  packageReqHandler.setParameter("total_fee", (am.Aconfiginfo.Vipmoney*100).ToString()); //商品金额,以分为单位(money * 100).ToString()
            packageReqHandler.setParameter("total_fee", "1");
            packageReqHandler.setParameter("trade_type", "APP");

            packageReqHandler.setParameter("attach", "");//自定义参数 127字符

            string Sign = packageReqHandler.CreateMd5Sign("key", AppKey);
            packageReqHandler.setParameter("sign", Sign);
            string data = packageReqHandler.parseXML();
            string prepayXml = HttpUtil.Send(data, "https://api.mch.weixin.qq.com/pay/unifiedorder");


            var xdoc = new XmlDocument();
            xdoc.LoadXml(prepayXml);
            XmlNode xn = xdoc.SelectSingleNode("xml");
            XmlNodeList xnl = xn.ChildNodes;
            if (xnl.Count > 7)
            {
                string PrepayId = xnl[7].InnerText;
                TimeStamp = getTimestamp();
                //随机字符串 
                NonceStr = getNoncestr();



                PayReq payRequest = new PayReq();
                payRequest.AppId = appid;
                payRequest.PrepayId = PrepayId;
                payRequest.PartnerId = mchid;
                payRequest.PackageValue = "Sign=WXPay";
                payRequest.NonceStr = NonceStr;
                payRequest.TimeStamp = TimeStamp;

                var paySignReqHandler = new RequestHandler();
                paySignReqHandler.setParameter("appid", payRequest.AppId);
                paySignReqHandler.setParameter("noncestr", payRequest.NonceStr);
                paySignReqHandler.setParameter("package", "Sign=WXPay");
                paySignReqHandler.setParameter("partnerid", payRequest.PartnerId);
                paySignReqHandler.setParameter("prepayid", payRequest.PrepayId);
                paySignReqHandler.setParameter("timestamp", payRequest.TimeStamp);
                // paySignReqHandler.setParameter("signType", "MD5");
                string PaySign = paySignReqHandler.CreateMd5Sign("key", AppKey);
                payRequest.Sign = PaySign;
                Toast.MakeText(this, "正在调起支付", ToastLength.Short).Show();
                wxApi.RegisterApp(appid);

              //这个返回 一直都是true 哪怕你参数都是错的 我也是醉了 我被这个状态迷惑了好久在找其他问题
                bool issceuss = wxApi.SendReq(payRequest);
                //  Toast.MakeText(this, "状态"+ issceuss, ToastLength.Short).Show();
            }
        }



        public static string getNoncestr()
        {
            Random random = new Random();
            return MD5Util.GetMD5(random.Next(1000).ToString(), "GBK");
        }
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static string getTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

    }
}