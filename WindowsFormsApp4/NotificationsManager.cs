﻿using System.Drawing;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace WindowsFormsApp4
{
    public class NotificationsManager
    {
        public static void Popup(string message, MessageBoxIcon icon)
        {
            PopupNotifier notifier = new PopupNotifier();
            notifier.TitleText = icon.ToString();
            notifier.TitleColor = Color.Black;
            notifier.TitleFont = new Font("Century Gothic", 15, FontStyle.Bold);
            notifier.ContentText = message;
            notifier.ContentColor = Color.Black;
            notifier.ContentFont = new Font("Century Gothic", 12);

            notifier.Popup();
        }
        
        public static void Popup(string message)
        {
            Popup(message, MessageBoxIcon.Error);
        }
    }
}
