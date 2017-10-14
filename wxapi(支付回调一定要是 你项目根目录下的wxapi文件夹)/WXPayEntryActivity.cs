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


using Com.Tencent.MM.Sdk.Openapi;
using Com.Tencent.MM.Sdk.Modelbase;
using Com.Tencent.MM.Sdk.Constants;

namespace com.jinjianlaowu.wxapi
{
    /// <summary>
    /// ΢�Ž�����
    /// </summary>
	// com.jinjianlaowu.wxapi.WXPayEntryActivit ��дȫ����İ��� Ҫ��Ȼ�޷��ص�
    [Android.App.Activity(Name = "com.jinjianlaowu.wxapi.WXPayEntryActivity", Exported = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]

    public class WXPayEntryActivity : Activity, IWXAPIEventHandler
    {
      
        protected override void OnCreate(Bundle bundle)
        { 
            base.OnCreate(bundle);
          //  SetContentView(Resource.Layout.pay_result);
            vipcompany.wxApi.HandleIntent(Intent, this);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Intent = intent;
            vipcompany.wxApi.HandleIntent(Intent, this);
        }

        /// <summary>
        ///  ����΢�ŷ�������Ϣ
        /// </summary>
        /// <param name="p0"></param>
        public void OnReq(BaseReq p0)
        {
         
            switch (p0.Type)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// ���ܷ���΢�ŵ���Ϣ�Ļص�
        /// </summary>
        /// <param name="p0"></param>
        public void OnResp(BaseResp p0)
        {
            int state = 0;
          
            if (p0.Type == ConstantsAPI.CommandPayByWx) {
                if (p0.MyErrCode == 0)
                {
			//֧���ɹ�
                    state = 1;
                }else{
			//ȡ��֧����֧��ʧ�� 
		
		        }
            
            }
          
            
        }
    }
}