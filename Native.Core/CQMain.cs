﻿using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code;
using top.riverelder.arkham.UI;
using Unity;

namespace Native.Core
{
	/// <summary>
	/// 酷Q应用主入口类
	/// </summary>
	public class CQMain
	{
		/// <summary>
		/// 在应用被加载时将调用此方法进行事件注册, 请在此方法里向 <see cref="IUnityContainer"/> 容器中注册需要使用的事件
		/// </summary>
		/// <param name="container">用于注册的 IOC 容器 </param>
		public static void Register (IUnityContainer unityContainer)
		{
            unityContainer.RegisterType<IAppEnable, Event_CqAppEnable>("应用已被启用");
            unityContainer.RegisterType<IGroupMessage, Event_GroupMessage>("群消息处理");
            unityContainer.RegisterType<IPrivateMessage, Event_PrivateMessage>("私聊消息处理");
            unityContainer.RegisterType<ICQExit, Event_CqExit>("酷Q关闭事件");
            unityContainer.RegisterType<IMenuCall, Menu_Chat>("打开聊天窗口");
        }
	}
}
