﻿#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2017 Jeffrey Su & Suzhou Senparc Network Technology Co.,Ltd.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.

Detail: https://github.com/JeffreySu/WeiXinMPSDK/blob/master/license.md

----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.Weixin.QY.CommonAPIs;
using Senparc.Weixin.QY.Entities;
using System.Threading.Tasks;
using Senparc.Weixin.QY.Containers;
using Senparc.Weixin.QY.Test.CommonApis;

namespace Senparc.Weixin.QY.Test.CommonAPIs
{
    //已测试通过
    [TestClass]
    public class AccessTokenContainerTest : CommonApiTest
    {
        [TestMethod]
        public void ContainerTest()
        {
            //注册
            AccessTokenContainer.Register(base._corpId, base._corpSecret);

            //获取Token完整结果（包括当前过期秒数）
            var tokenResult = AccessTokenContainer.GetTokenResult(base._corpId,base._corpSecret);
            Assert.IsNotNull(tokenResult);

            //只获取Token字符串
            var token = AccessTokenContainer.GetToken(base._corpId, base._corpSecret);
            Assert.AreEqual(tokenResult.access_token, token);

            //getNewToken
            {
                token = AccessTokenContainer.TryGetToken(base._corpId, base._corpSecret, false);
                Assert.AreEqual(tokenResult.access_token, token);

                token = AccessTokenContainer.TryGetToken(base._corpId, base._corpSecret, true);
                Assert.AreEqual(tokenResult.access_token, token);//现在微信服务器有AccessToken缓存，短时间内一致
            }
        }

        [TestMethod]
        public void GetTokenResultTest()
        {
            //注册
            AccessTokenContainer.Register(base._corpId, base._corpSecret);

            //模拟多线程获取
            List<string> accessTokenList = new List<string>();
            int[] treads = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Parallel.For(0, treads.Length, (i) =>
            {
                var accessTokenResult = AccessTokenContainer.GetTokenResult(base._corpId, base._corpSecret, false);
                accessTokenList.Add(accessTokenResult.access_token);//同时多次获取
            });

            Assert.AreEqual(treads.Length, accessTokenList.Count());//只存在同一个Token，实际不会多次刷新
            Assert.AreEqual(1, accessTokenList.Distinct().Count());//只存在同一个Token，实际不会多次刷新
            Console.WriteLine(accessTokenList[0]);
        }
    }
}
