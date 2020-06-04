using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.UI {
    public class Menu_Chat : IMenuCall {

        private MainWindow mainWindow = null;

        public void MenuCall(object sender, CQMenuCallEventArgs e) {
            if (this.mainWindow == null) {
                this.mainWindow = new MainWindow();
                this.mainWindow.Closing += MainWindow_Closing;
                this.mainWindow.Show();	// 显示窗体
            } else {
                this.mainWindow.Activate();	// 将窗体调制到前台激活
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // 对变量置 null, 因为被关闭的窗口无法重复显示
            this.mainWindow = null;
        }
    }
}
